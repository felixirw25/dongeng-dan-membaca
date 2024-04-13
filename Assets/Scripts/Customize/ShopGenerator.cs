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
using System.Linq;

public class ShopGenerator : MonoBehaviour
{
    [SerializeField] private GameObject warningPanel;
    FirebaseAuth auth;
    FirebaseFirestore firestore;
    public List<Pakaian> listPakaianInfo;
    GameObject kontenPakaian;
    public GameObject pakaianPrefab;
    [SerializeField] public GameObject cover;
    public UserData userData;
    public int showBintang = 0;
    public List<GameObject> shopPanels;
    public Text starsText;
    [SerializeField] CekItemPakaian cekItemPakaian;

    private void Awake(){
        auth = FirebaseAuth.DefaultInstance;
        firestore = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
    }

    [Obsolete]
    private void Start(){
        userData = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>();
        kontenPakaian = GameObject.FindGameObjectWithTag("KontenPakaian");

        listPakaianInfo = new List<Pakaian>();

        firestore.Collection("accessories").GetSnapshotAsync().ContinueWithOnMainThread(task=>{
            QuerySnapshot queryDocuments = task.Result;

            foreach(DocumentSnapshot document in queryDocuments.Documents){
                Dictionary<string, object> pakaian = document.ToDictionary();
                float xscale = Convert.ToSingle(pakaian["xscale"]);
                float yscale = Convert.ToSingle(pakaian["yscale"]);
                Pakaian pakaianBaruInfo = new Pakaian(
                document.Id, 
                (string)pakaian["gambar"], 
                (string)pakaian["harga"], 
                (string)pakaian["nama"],
                (string)pakaian["tipe"],
                (string)pakaian["x"],
                (string)pakaian["y"],
                xscale,
                yscale);
                listPakaianInfo.Add(pakaianBaruInfo);
            }

            // kontenPakaian = GameObject.FindGameObjectWithTag("KontenPakaian");
            foreach (Pakaian pakaian in listPakaianInfo)
            {
                GameObject pakaianBaru = Instantiate(pakaianPrefab, kontenPakaian.transform) as GameObject;
                pakaianBaru.GetComponent<ShopTemplate>().itemId = pakaian.Id_pakaian;
                pakaianBaru.GetComponent<ShopTemplate>().namaTxt.SetText(pakaian.Nama);
                pakaianBaru.GetComponent<ShopTemplate>().hargaTxt.SetText(pakaian.Harga.ToString());
                pakaianBaru.GetComponent<ShopTemplate>().tipe = pakaian.Tipe;
                pakaianBaru.GetComponent<ShopTemplate>().xpos = pakaian.Xpos;
                pakaianBaru.GetComponent<ShopTemplate>().ypos = pakaian.Ypos;
                pakaianBaru.GetComponent<ShopTemplate>().xscale = pakaian.Xscale;
                pakaianBaru.GetComponent<ShopTemplate>().yscale = pakaian.Yscale;
                StartCoroutine(LoadImage(pakaian.Gambar.ToString(), pakaianBaru));
            }
        });

        StartCoroutine(ReadData(4f));            
    }

    public void CloseWarningSign(){
        warningPanel.SetActive(false);
    }

    [Obsolete]
    IEnumerator LoadImage(string mediaUrl, GameObject pakaianBaru){
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest(); 
        if(request.isNetworkError || request.isHttpError){
            Debug.Log(request.error);
        }
        else{
            pakaianBaru.GetComponent<ShopTemplate>().gambar.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
    public IEnumerator LoadCover(float second){
        cover.SetActive(true); 
        yield return new WaitForSecondsRealtime(second); 
        cover.SetActive(false);
    }

    public IEnumerator ReadData(float second){
        cover.SetActive(true);
        userData.ReadUserDataAsync();
        yield return new WaitForSecondsRealtime(second);
        firestore.Collection("usersData").Document(auth.CurrentUser.UserId.Trim().ToString()).Collection("progressAccessories").GetSnapshotAsync().ContinueWithOnMainThread(task=>{
            QuerySnapshot queryDocuments = task.Result;
            List<DocumentSnapshot> listOwned = new List<DocumentSnapshot>();

            userData.newPlayerProfile.pakaianOwned = new List<DocumentSnapshot>();

            foreach(DocumentSnapshot document in queryDocuments.Documents){
                Debug.Log("document.ToDictionary()[isUnlocked].ToString(): " + document.ToDictionary()["isUnlocked"].ToString());
                Debug.Log("document.ToDictionary()isUnlocked.ToString().Equals(true): " + document.ToDictionary()["isUnlocked"].ToString().Equals("true"));
                if(document.ToDictionary()["isUnlocked"].ToString().Trim().Equals("true")){
                    Debug.Log("document.Id: " + document.Id);
                    listOwned.Add(document);
                }
            }

            foreach (var pakaian in listPakaianInfo)
            {
                foreach (var pakaianOwned in listOwned)
                {
                    if(pakaian.Id_pakaian.ToString().Trim().Equals(pakaianOwned.Id.ToString().Trim())){
                        GameObject pakaianPrefab = kontenPakaian.transform.GetChild(listPakaianInfo.IndexOf(pakaian)).gameObject;
                        pakaianPrefab.transform.GetChild(2).gameObject.SetActive(false);
                        pakaianPrefab.transform.GetChild(3).gameObject.SetActive(true);
                        pakaianPrefab.transform.GetChild(4).gameObject.SetActive(false);
                        pakaianPrefab.transform.GetChild(6).gameObject.SetActive(false);
                    }
                    if(pakaian.Id_pakaian.ToString().Trim().Equals(PlayerPrefs.GetString("lastEquippedPakaian", "")) || pakaian.Id_pakaian.ToString().Trim().Equals(PlayerPrefs.GetString("lastEquippedAcc", ""))){
                        GameObject pakaianPrefab = kontenPakaian.transform.GetChild(listPakaianInfo.IndexOf(pakaian)).gameObject;
                        pakaianPrefab.transform.GetChild(2).gameObject.SetActive(true);
                        pakaianPrefab.transform.GetChild(3).gameObject.SetActive(false);
                        pakaianPrefab.transform.GetChild(4).gameObject.SetActive(false);
                        pakaianPrefab.transform.GetChild(6).gameObject.SetActive(false);
                    }

                    // Additional check for pakaian and accessories not in listOwned and matching lastEquippedPakaian
                    if (!listOwned.Any(p => p.Id.ToString().Trim().Equals(pakaian.Id_pakaian.ToString().Trim())) &&
                        pakaian.Id_pakaian.ToString().Trim().Equals(PlayerPrefs.GetString("lastEquippedPakaian", "")))
                    {
                        PlayerPrefs.SetString("lastEquippedPakaian", ""); // Set to an empty string
                    }

                    if (!listOwned.Any(p => p.Id.ToString().Trim().Equals(pakaian.Id_pakaian.ToString().Trim())) &&
                        pakaian.Id_pakaian.ToString().Trim().Equals(PlayerPrefs.GetString("lastEquippedAcc", "")))
                    {
                        PlayerPrefs.SetString("lastEquippedAcc", ""); // Set to an empty string
                    }
                }
            } 
        });
        StartCoroutine(LoadCover(2f));
    }
    void Update(){
        showBintang = userData.newPlayerProfile.CountStarAll()-userData.newPlayerProfile.StarSpentCount;
        starsText.text = showBintang.ToString();
    }
}
