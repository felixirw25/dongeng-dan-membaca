using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Storage;

public class SceneLoader : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseFirestore firestore;
    FirebaseStorage storage;
    [SerializeField] private GameObject noSignal;
    [SerializeField] AudioClip mainMenuClip;
    [SerializeField] public GameObject cover;
    [SerializeField] public GameObject konfirmasiPanel;
    [SerializeField] private bool yakin = false;
    [SerializeField] AcakKataManager acakKataManager;
    public bool Yakin { get => yakin; set => yakin = value; }
    [SerializeField] bool buttonClicked;
    [SerializeField] public bool ButtonClicked { get => buttonClicked; set => buttonClicked = value; }
    public Text konfirmasiText;
    public GameObject mainMenu;
    public GameObject settingMenu;
    public GameObject aboutMenu;

    private void Start()
    {
        noSignal.SetActive(false);

        if(mainMenu != null || settingMenu != null || aboutMenu != null){
            mainMenu.SetActive(true);
            settingMenu.SetActive(false);
            aboutMenu.SetActive(false);
        }

        if(GameObject.FindGameObjectWithTag("InfoPanelKonfirmasi")!=null){
            konfirmasiPanel = GameObject.FindGameObjectWithTag("InfoPanelKonfirmasi");
            konfirmasiPanel.SetActive(false);
            konfirmasiText.text = "";
        }
    }

    private void Update(){
        if(Application.internetReachability == NetworkReachability.NotReachable){
            noSignal.SetActive(true);
        }
        else{
            noSignal.SetActive(false);
        }
    }

    public void LoadScene(string sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void Exit(string sceneIndex){
        if(acakKataManager!=null)
            acakKataManager.Indeks = 0;
        var audioManager = GameObject.Find("AudioManager");
        audioManager.GetComponent<AudioSource>().clip = mainMenuClip;
        audioManager.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame(){
        StartCoroutine(QuitGameIE());
    }

    public IEnumerator QuitGameIE(){
        konfirmasiText.text = "Yakin ingin keluar?";
        konfirmasiPanel.SetActive(true);
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
            yield return null;
        }
        if(Yakin){
            Application.Quit();
        }
        else{
            konfirmasiText.text = "";
            konfirmasiPanel.SetActive(false);
            yield break;
        }
    }
    public IEnumerator LoadCover(float second){
        cover.SetActive(true); 
        yield return new WaitForSecondsRealtime(second); 
        cover.SetActive(false);
    }

    public void SettingGame(){
        settingMenu.SetActive(true);
        mainMenu.SetActive(false);
        aboutMenu.SetActive(false);
    }
    public void BackMenu(){
        mainMenu.SetActive(true);
        settingMenu.SetActive(false);
        aboutMenu.SetActive(false);
    }
    public void AboutMenu(){
        aboutMenu.SetActive(true);
        settingMenu.SetActive(false);
        mainMenu.SetActive(false);
    }
    public void BackSetting(){
        aboutMenu.SetActive(false);
        settingMenu.SetActive(true);
    }
    public void GRDestruct(){
        if(GameObject.FindGameObjectWithTag("GameResource")!=null){
            Destroy(GameObject.FindGameObjectWithTag("GameResource"));
        }
        if(GameObject.FindGameObjectWithTag("UserData")!=null){
            Destroy(GameObject.FindGameObjectWithTag("UserData"));
        }
    }
}
