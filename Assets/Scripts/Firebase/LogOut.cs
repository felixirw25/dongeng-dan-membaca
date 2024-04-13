using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Google;

public class LogOut : MonoBehaviour
{
    [SerializeField] SceneLoader sceneLoader;
    FirebaseAuth auth;
    // private void Awake(){
    //     auth = FirebaseAuth.DefaultInstance;
    // }
    private void Start(){
        sceneLoader.konfirmasiPanel.SetActive(false);
        sceneLoader.konfirmasiText.text = "";
    }
    public void LogOutUser(){
        auth = FirebaseAuth.DefaultInstance;
        StartCoroutine(LogOutUserIE());
    }
    public IEnumerator LogOutUserIE(){
        sceneLoader.konfirmasiPanel.SetActive(true);
        sceneLoader.ButtonClicked = false;
        sceneLoader.konfirmasiText.text = "Yakin ingin ganti akun?";

        // You can replace the next line with a reference to your "Yes" button
        Button yesButton = GameObject.FindGameObjectWithTag("YesButton").GetComponent<Button>();
        
        // You can replace the next line with a reference to your "No" button
        Button noButton = GameObject.FindGameObjectWithTag("NoButton").GetComponent<Button>();

        yesButton.onClick.AddListener(() => {
            sceneLoader.ButtonClicked = true;
            sceneLoader.Yakin = true;
        });

        noButton.onClick.AddListener(() => {
            sceneLoader.ButtonClicked = true;
            sceneLoader.Yakin = false;
        });
        
        while (!sceneLoader.ButtonClicked)
        {
            yield return null;
        }
        if(sceneLoader.Yakin){
            if(PlayerPrefs.GetString("AuthType", "")=="Google")
                GoogleSignIn.DefaultInstance.SignOut();
            auth.SignOut();
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Destroy(GameObject.FindGameObjectWithTag("GameResource"));
            Destroy(GameObject.FindGameObjectWithTag("UserData"));
            Debug.Log("PlayerPrefs have been reset.");
            SceneManager.LoadScene("LoginSignup");
        }
        else{
            sceneLoader.konfirmasiText.text = "";
            sceneLoader.konfirmasiPanel.SetActive(false);
            yield break;
        }
    }
}
