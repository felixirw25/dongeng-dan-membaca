using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Storage;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Globalization;

public class CekItemPakaian : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseFirestore firestore;
    UserData userData;
    public PlayerProfile newPlayerProfile;
    [SerializeField] private GameObject warningPanel;
    int star;
    int starSpent;
    [SerializeField] GameObject panelBeli;
    [SerializeField] ShopTemplate item;
    [SerializeField] Dictionary<string, object> updateStarSpent;
    [SerializeField] Dictionary<string, object> itemPakaian;

    [SerializeField] private bool yakin = false;
    public bool Yakin { get => yakin; set => yakin = value; }
    [SerializeField] bool buttonClicked;
    [SerializeField] public bool ButtonClicked { get => buttonClicked; set => buttonClicked = value; }

    private void Awake(){
        auth = FirebaseAuth.DefaultInstance;
        firestore = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
        warningPanel = GameObject.FindGameObjectWithTag("InfoPanel");
        panelBeli = GameObject.FindGameObjectWithTag("InfoPanelBeli");
    }

    private void Start(){
        warningPanel.SetActive(false);
        panelBeli.SetActive(false);
        ShopTemplate item = gameObject.GetComponentInParent<ShopTemplate>();
        if(item.itemId==PlayerPrefs.GetString("lastEquippedPakaian", "")){
            GameObject abenkPolos = GameObject.FindGameObjectWithTag("AbenkPolos");
            GameObject prefabGambar = new GameObject();
            prefabGambar.tag = PlayerPrefs.GetString("lastEquippedPakaianTipe", "");

            float xPos = float.Parse(PlayerPrefs.GetString("lastEquippedPakaianXPos", ""));
            float yPos = float.Parse(PlayerPrefs.GetString("lastEquippedPakaianYPos", ""));
            Vector2 positionItem = new Vector2(xPos, yPos);
            prefabGambar.GetComponent<Transform>().localPosition = positionItem;

            float xScale = PlayerPrefs.GetFloat("lastEquippedPakaianXScale", 0);
            float yScale = PlayerPrefs.GetFloat("lastEquippedPakaianYScale", 0);
            Vector3 scaleItem = new Vector3((float)xScale, (float)yScale, 1f);
            prefabGambar.GetComponent<Transform>().localScale = scaleItem;

            byte[] bytes = System.Convert.FromBase64String(PlayerPrefs.GetString("lastEquippedPakaianGambar", "")); // Convert from base64 string
            Texture2D texture = new Texture2D(2, 2); // Create a new Texture2D
            texture.LoadImage(bytes);

            prefabGambar.AddComponent<RawImage>();
            prefabGambar.GetComponent<RawImage>().texture = texture;

            GameObject pakaianTerpakai = Instantiate(prefabGambar, abenkPolos.transform) as GameObject;
        }
        if(item.itemId==PlayerPrefs.GetString("lastEquippedAcc", "")){
            GameObject abenkPolos = GameObject.FindGameObjectWithTag("AbenkPolos");
            GameObject prefabGambar = new GameObject();
            prefabGambar.tag = PlayerPrefs.GetString("lastEquippedAccTipe", "");

            float xPos = float.Parse(PlayerPrefs.GetString("lastEquippedAccXPos", ""));
            float yPos = float.Parse(PlayerPrefs.GetString("lastEquippedAccYPos", ""));
            Vector2 positionItem = new Vector2(xPos, yPos);
            prefabGambar.GetComponent<Transform>().localPosition = positionItem;

            float xScale = PlayerPrefs.GetFloat("lastEquippedAccXScale", 0);
            float yScale = PlayerPrefs.GetFloat("lastEquippedAccYScale", 0);
            Vector3 scaleItem = new Vector3((float)xScale, (float)yScale, 1f);
            prefabGambar.GetComponent<Transform>().localScale = scaleItem;

            byte[] bytes = System.Convert.FromBase64String(PlayerPrefs.GetString("lastEquippedAccGambar", "")); // Convert from base64 string
            Texture2D texture = new Texture2D(2, 2); // Create a new Texture2D
            texture.LoadImage(bytes);

            prefabGambar.AddComponent<RawImage>();
            prefabGambar.GetComponent<RawImage>().texture = texture;

            GameObject pakaianTerpakai = Instantiate(prefabGambar, abenkPolos.transform) as GameObject;
        }
    }

    public async void BtnBeli(){
        item = gameObject.GetComponentInParent<ShopTemplate>();
        int hargaItem = Int32.Parse(gameObject.GetComponentInParent<ShopTemplate>().hargaTxt.text.ToString());
        await firestore.Collection("usersData").Document(auth.CurrentUser.UserId).GetSnapshotAsync().ContinueWithOnMainThread(task=>{
            Dictionary<string, object> userInfo = task.Result.ToDictionary();
            star = Convert.ToInt32(userInfo["Star"]);
            starSpent = Convert.ToInt32(userInfo["StarSpent"]);

            if(star-starSpent>=hargaItem){
                panelBeli.SetActive(true);
                StartCoroutine(Beli());
                // await firestore.Collection("usersData").Document(auth.CurrentUser.UserId).UpdateAsync(updateStarSpent);
                // await firestore.Collection("usersData").Document(auth.CurrentUser.UserId).Collection("progressAccessories").Document(item.itemId.ToString().Trim()).UpdateAsync(itemPakaian).ContinueWithOnMainThread(task=>{
                //     item.transform.GetChild(3).gameObject.SetActive(true);
                //     item.transform.GetChild(4).gameObject.SetActive(false);
                //     item.transform.GetChild(6).gameObject.SetActive(false);
                //     userData.newPlayerProfile.StarSpentCount+=Int32.Parse(item.hargaTxt.text);
                // });
                // panelBeli.SetActive(false);
            }
            else{
                Debug.Log("Bintang Tidak Cukup!");
                warningPanel.SetActive(true);
            }
        });
    }

    public IEnumerator Beli(){
        buttonClicked = false;

        // You can replace the next line with a reference to your "Yes" button
        Button yesButton = GameObject.FindGameObjectWithTag("YesButton").GetComponent<Button>();
        
        // You can replace the next line with a reference to your "No" button
        Button noButton = GameObject.FindGameObjectWithTag("NoButton").GetComponent<Button>();

        yesButton.onClick.AddListener(() => {
            buttonClicked = true;
            Yakin = true;
        });

        noButton.onClick.AddListener(() => {
            buttonClicked = true;
            Yakin = false;
        });
        
        while (!buttonClicked)
        {
            yield return null; // Yielding null lets the coroutine wait for the next frame
        }
        if(Yakin){
            ShopGenerator shopGenerator = GameObject.FindGameObjectWithTag("ShopGenerator").GetComponent<ShopGenerator>();
            shopGenerator.userData.newPlayerProfile.StarSpentCount+=Int32.Parse(item.hargaTxt.text);

            itemPakaian = new Dictionary<string, object>();
            itemPakaian.Add("isUnlocked", "true");

            updateStarSpent = new Dictionary<string, object>();
            updateStarSpent.Add("StarSpent", starSpent+Int32.Parse(item.hargaTxt.text));
            firestore.Collection("usersData").Document(auth.CurrentUser.UserId).UpdateAsync(updateStarSpent);
            firestore.Collection("usersData").Document(auth.CurrentUser.UserId).Collection("progressAccessories").Document(item.itemId.ToString().Trim()).UpdateAsync(itemPakaian).ContinueWithOnMainThread(task=>{
                item.transform.GetChild(3).gameObject.SetActive(true);
                item.transform.GetChild(4).gameObject.SetActive(false);
                item.transform.GetChild(6).gameObject.SetActive(false);
                userData.newPlayerProfile.StarSpentCount+=Int32.Parse(item.hargaTxt.text);
            });
            buttonClicked = false;
            Yakin = false;
            yield return new WaitForSecondsRealtime(1);
            panelBeli.SetActive(false);
        }
        else{
            buttonClicked = false;
            Yakin = false;
            panelBeli.SetActive(false);
        }
    }

    public void btnPakai(){
        ShopTemplate item = gameObject.GetComponentInParent<ShopTemplate>();
        GameObject abenkPolos = GameObject.FindGameObjectWithTag("AbenkPolos");
        GameObject prefabGambar = new GameObject();
        prefabGambar.tag = item.tipe;

        float xPos = float.Parse(item.xpos);
        float yPos = float.Parse(item.ypos);
        Vector2 positionItem = new Vector2(xPos, yPos);
        prefabGambar.GetComponent<Transform>().localPosition = positionItem;

        float xScale = item.xscale;
        float yScale = item.yscale;
        Vector3 scaleItem = new Vector3((float)xScale, (float)yScale, 1f);
        prefabGambar.GetComponent<Transform>().localScale = scaleItem;

        prefabGambar.AddComponent<RawImage>();
        prefabGambar.GetComponent<RawImage>().texture = item.gambar.texture;
        lepasSemua(item.tipe);
        item.transform.GetChild(2).gameObject.SetActive(true);
        item.transform.GetChild(3).gameObject.SetActive(false);
        for (int i = 0; i < abenkPolos.transform.childCount; i++){
            if(abenkPolos.transform.GetChild(i).tag==prefabGambar.tag){
                Destroy(abenkPolos.transform.GetChild(i).gameObject);
            }
        }
        GameObject pakaianTerpakai = Instantiate(prefabGambar, abenkPolos.transform) as GameObject;

        if(prefabGambar.tag=="baju"){
            PlayerPrefs.SetString("lastEquippedPakaian", item.itemId);
            PlayerPrefs.SetString("lastEquippedPakaianTipe", item.tipe);
            PlayerPrefs.SetString("lastEquippedPakaianXPos", item.xpos);
            PlayerPrefs.SetString("lastEquippedPakaianYPos", item.ypos);
            PlayerPrefs.SetFloat("lastEquippedPakaianXScale", item.xscale);
            PlayerPrefs.SetFloat("lastEquippedPakaianYScale", item.yscale);

            // Create a new Texture2D with the same dimensions as the source texture
            Texture2D targetTexture = new Texture2D(item.gambar.texture.width, item.gambar.texture.height);

            // Copy the pixels from the source texture to the target texture
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = RenderTexture.GetTemporary(item.gambar.texture.width, item.gambar.texture.height);
            Graphics.Blit(item.gambar.texture, renderTexture);
            RenderTexture.active = renderTexture;
            targetTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            targetTexture.Apply();
            RenderTexture.active = currentRT;

            // Release the temporary render texture
            RenderTexture.ReleaseTemporary(renderTexture);

            byte[] bytes = targetTexture.EncodeToPNG(); // Encode the texture to PNG format
            string encodedTexture = System.Convert.ToBase64String(bytes); // Convert to a base64 string
            PlayerPrefs.SetString("lastEquippedPakaianGambar", encodedTexture);

            PlayerPrefs.Save();
            Debug.Log("lastEquippedPakaian: " + PlayerPrefs.GetString("lastEquippedPakaian", ""));
        }
        else{
            PlayerPrefs.SetString("lastEquippedAcc", item.itemId);
            PlayerPrefs.SetString("lastEquippedAccTipe", item.tipe);
            PlayerPrefs.SetString("lastEquippedAccXPos", item.xpos);
            PlayerPrefs.SetString("lastEquippedAccYPos", item.ypos);
            PlayerPrefs.SetFloat("lastEquippedAccXScale", item.xscale);
            PlayerPrefs.SetFloat("lastEquippedAccYScale", item.yscale);

            // Create a new Texture2D with the same dimensions as the source texture
            Texture2D targetTexture = new Texture2D(item.gambar.texture.width, item.gambar.texture.height);

            // Copy the pixels from the source texture to the target texture
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = RenderTexture.GetTemporary(item.gambar.texture.width, item.gambar.texture.height);
            Graphics.Blit(item.gambar.texture, renderTexture);
            RenderTexture.active = renderTexture;
            targetTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            targetTexture.Apply();
            RenderTexture.active = currentRT;

            // Release the temporary render texture
            RenderTexture.ReleaseTemporary(renderTexture);

            byte[] bytes = targetTexture.EncodeToPNG(); // Encode the texture to PNG format
            string encodedTexture = System.Convert.ToBase64String(bytes); // Convert to a base64 string
            PlayerPrefs.SetString("lastEquippedAccGambar", encodedTexture);
            PlayerPrefs.Save();
            Debug.Log("lastEquippedAcc: " + PlayerPrefs.GetString("lastEquippedAcc", ""));
        }
    }

    public void btnLepas(){
        ShopTemplate item = gameObject.GetComponentInParent<ShopTemplate>();
        GameObject abenkPolos = GameObject.FindGameObjectWithTag("AbenkPolos");
        GameObject prefabGambar = new GameObject();
        prefabGambar.tag = item.tipe;
        item.transform.GetChild(2).gameObject.SetActive(false);
        item.transform.GetChild(3).gameObject.SetActive(true);
        for (int i = 0; i < abenkPolos.transform.childCount; i++){
            if(abenkPolos.transform.GetChild(i).tag==prefabGambar.tag){
                Destroy(abenkPolos.transform.GetChild(i).gameObject);
            }
        }
        if(item.tipe=="baju"){
            PlayerPrefs.SetString("lastEquippedPakaian", "");
            PlayerPrefs.SetString("lastEquippedPakaianTipe", "");
            PlayerPrefs.SetString("lastEquippedPakaianXPos", "");
            PlayerPrefs.SetString("lastEquippedPakaianYPos", "");
            PlayerPrefs.SetFloat("lastEquippedPakaianXScale", 0);
            PlayerPrefs.SetFloat("lastEquippedPakaianYScale", 0);
            PlayerPrefs.SetString("lastEquippedPakaianGambar", "");
            PlayerPrefs.Save();
        }
        else{
            PlayerPrefs.SetString("lastEquippedAcc", "");
            PlayerPrefs.SetString("lastEquippedAccTipe", "");
            PlayerPrefs.SetString("lastEquippedAccXPos", "");
            PlayerPrefs.SetString("lastEquippedAccYPos", "");
            PlayerPrefs.SetFloat("lastEquippedAccXScale", 0);
            PlayerPrefs.SetFloat("lastEquippedAccYScale", 0);
            PlayerPrefs.SetString("lastEquippedAccGambar", "");
            PlayerPrefs.Save();
        }
    }

    public void lepasSemua(String tipeItem){
        GameObject itemTerpakai = GameObject.FindGameObjectWithTag("KontenPakaian");
        for (int i = 0; i < itemTerpakai.transform.childCount; i++)
        {
            Transform transformItem = itemTerpakai.transform.GetChild(i);
            if (!transformItem.GetChild(6).gameObject.activeInHierarchy &&
                transformItem.GetComponent<ShopTemplate>().tipe==tipeItem)
            {
                itemTerpakai.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
                itemTerpakai.transform.GetChild(i).GetChild(3).gameObject.SetActive(true);
            }
        }
    }
}
