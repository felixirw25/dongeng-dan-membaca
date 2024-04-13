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
using System.Linq;

public class SceneLoaderCerita : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseFirestore firestore;
    FirebaseStorage storage;
    [SerializeField] private GameObject noSignal;
    [SerializeField] public GameObject cover;
    [SerializeField] AudioClip mainMenuClip;
    public GameObject levelSelectionPrefab;
    public LevelSelector levelSelector;
    public LevelSelection levelSelection;
    public UIManager uiManager;
    [Header("Instantiate Cerita Dinamis")]
    public GameObject parent;
    public GameObject listCerita;
    public List<GameObject> listCeritas;
    public List<Cerita> listCeritaInfo;
    public GameObject prefabCerita;
    public QuerySnapshot queryDocuments;

    [Obsolete]
    private async void Awake(){
        auth = FirebaseAuth.DefaultInstance;
        firestore = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
        storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        int counter = 1;

        await firestore.Collection("stories").OrderBy("id_cerita").GetSnapshotAsync().ContinueWithOnMainThread(async task=>{
            queryDocuments = task.Result;

            listCeritaInfo = new List<Cerita>();
            queryDocuments.Documents.OrderBy(c=>c.ToDictionary()["id_cerita"]);
            foreach(DocumentSnapshot document in queryDocuments.Documents){
                GameObject ceritaBaru;
                Dictionary<string, object> cerita = document.ToDictionary();
                ceritaBaru = Instantiate(prefabCerita, listCerita.transform) as GameObject;
                ceritaBaru.GetComponent<ChooseStory>().indexNumber = counter;
                if(counter==1){
                    StartCoroutine(LoadImage(cerita["thumbnail"].ToString(), ceritaBaru));
                    ceritaBaru.GetComponent<Button>().interactable = true;
                    ceritaBaru.transform.GetChild(0).gameObject.SetActive(true);
                    ceritaBaru.transform.GetChild(0).GetComponent<Text>().text = cerita["title"].ToString();
                    ceritaBaru.transform.GetChild(1).gameObject.SetActive(true);
                    ceritaBaru.transform.GetChild(1).GetComponent<Text>().text = cerita["type"].ToString();
                    counter++;
                }
                else{
                    ceritaBaru.GetComponent<Button>().interactable = false;
                    ceritaBaru.transform.GetChild(0).gameObject.SetActive(false);
                    ceritaBaru.transform.GetChild(1).gameObject.SetActive(false);

                    if (listCeritaInfo.Count!=0){
                        await firestore.Collection("usersData").Document(auth.CurrentUser.UserId.Trim().ToString()).Collection("progressCerita").OrderBy("id_cerita").GetSnapshotAsync().ContinueWithOnMainThread(taskLevel=>{
                            QuerySnapshot queryLevelDocuments = taskLevel.Result;
                            queryLevelDocuments.Documents.OrderBy(c=>c.ToDictionary()["id_cerita"]);
                            foreach(DocumentSnapshot userProgressDocument in queryLevelDocuments.Documents){
                                if(userProgressDocument.Id.Trim().Equals(listCeritaInfo[counter-2].Id_dokumen)){
                                    Dictionary<string, object> userProgressData = userProgressDocument.ToDictionary();
                                    if(userProgressData["isFinished"].ToString().Trim().Equals("true")){
                                        StartCoroutine(LoadImage(cerita["thumbnail"].ToString(), ceritaBaru));
                                        ceritaBaru.GetComponent<Button>().interactable = true;
                                        ceritaBaru.transform.GetChild(0).gameObject.SetActive(true);
                                        ceritaBaru.transform.GetChild(0).GetComponent<Text>().text = cerita["title"].ToString();
                                        ceritaBaru.transform.GetChild(1).gameObject.SetActive(true);
                                        ceritaBaru.transform.GetChild(1).GetComponent<Text>().text = cerita["type"].ToString();
                                        ceritaBaru.GetComponent<ChooseStory>().indexNumber = counter;
                                    }
                                    break;
                                }
                            }
                            counter++;
                        });    
                    }
                }
                listCeritas.Add(ceritaBaru);
                
                long idCerita = (long)cerita["id_cerita"];
                long variasi_adegan = (long)cerita["variasi_adegan"];
                Cerita ceritaBaruInfo = new Cerita(document.Id, 
                (int)idCerita,
                (string)cerita["thumbnail"], 
                (string)cerita["title"], 
                (string)cerita["type"],
                (int)variasi_adegan);
                listCeritaInfo.Add(ceritaBaruInfo);    
            }
        });
        listCeritas.OrderBy(c=>c.GetComponent<ChooseStory>().indexNumber);
        listCeritaInfo.OrderBy(c=>c.Id_cerita);
    }

    private void Start()
    {
        StartCoroutine(LoadCover(4f));
        noSignal.SetActive(false);
        uiManager = GameObject.FindGameObjectWithTag("UIManager")?.GetComponent<UIManager>();

        if(parent==null)
            return;
        else{
            levelSelector = GameObject.FindGameObjectWithTag("LevelSelector")?.GetComponent<LevelSelector>();
        }
        StartCoroutine(SpawnLevel());
    }
    private void Update(){
        if(Application.internetReachability == NetworkReachability.NotReachable){
            noSignal.SetActive(true);
        }
        else{
            noSignal.SetActive(false);
        }
    }

    [Obsolete]
    IEnumerator LoadImage(string mediaUrl, GameObject ceritaBaru){
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest(); 
        if(request.isNetworkError || request.isHttpError){
            Debug.Log(request.error);
        }
        else{
            ceritaBaru.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    public IEnumerator LoadCover(float second){
        cover.SetActive(true); 
        yield return new WaitForSecondsRealtime(second); 
        cover.SetActive(false);
    }

    private IEnumerator SpawnLevel()
    {        
        foreach (Transform child in parent.transform)
        {
            Destroy(child);
        }

        UserData userData = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>();
        userData.ReadUserDataAsync();
        StartCoroutine(LoadCover(3f));
        yield return new WaitForSecondsRealtime(3f);
        // for(int i = 0; i < Mathf.FloorToInt((float)levelSelector.sceneStoryCollections.Count/3); i++){
        for(int i = 0; i < Mathf.FloorToInt((float)levelSelector.sceneStoryCollections.Count/levelSelector.variasiLevel); i++){
            if(i < levelSelector.sceneQuizCollections.Count){
                // int index = i*3;
                int index = i*levelSelector.variasiLevel;
                GameObject spawnedLevelButton = Instantiate(levelSelectionPrefab, Vector3.zero, Quaternion.identity, parent.transform);
                
                PlayerProfile data = userData.newPlayerProfile;

                if(i == 0){
                    spawnedLevelButton.transform.GetChild(0).gameObject.SetActive(false);
                    spawnedLevelButton.transform.GetChild(1).gameObject.SetActive(true);
                    spawnedLevelButton.transform.GetChild(2).gameObject.SetActive(true);
                    spawnedLevelButton.transform.GetChild(3).gameObject.SetActive(true);
                }
                else{
                    if (data.starPerLevelCerita[levelSelector.ceritaTerpilih.Id_cerita-1][i-1] > 0){
                        spawnedLevelButton.transform.GetChild(0).gameObject.SetActive(false);
                        spawnedLevelButton.transform.GetChild(1).gameObject.SetActive(true);
                        spawnedLevelButton.transform.GetChild(2).gameObject.SetActive(true);
                        spawnedLevelButton.transform.GetChild(3).gameObject.SetActive(true);
                    }
                    else{
                        spawnedLevelButton.transform.GetChild(0).gameObject.SetActive(true);
                        spawnedLevelButton.transform.GetChild(1).gameObject.SetActive(false);
                        spawnedLevelButton.transform.GetChild(2).gameObject.SetActive(false);
                        spawnedLevelButton.transform.GetChild(3).gameObject.SetActive(false);
                    }   
                }
                if(spawnedLevelButton.transform.GetChild(0).gameObject.activeInHierarchy){
                    spawnedLevelButton.GetComponent<Button>().interactable = false;
                }
                else{
                    spawnedLevelButton.GetComponent<Button>().interactable = true;
                }
                spawnedLevelButton.transform.GetChild(4).gameObject.GetComponent<Text>().text = levelSelector.ceritaTerpilih.Id_cerita+"-"+(i+1).ToString();
                spawnedLevelButton.GetComponent<Button>().onClick.AddListener(()=>{
                    levelSelector.levelSelected = index;
                    LoadScene("Kuis");
                });
                levelSelection.levels.Add(spawnedLevelButton);
            }
        }
        levelSelection.UpdateLevelImage();
        uiManager.UpdateStarsUI();
    }

    public void LoadScene(string sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void Exit(string sceneIndex){
        Destroy(levelSelector.gameObject);
        var audioManager = GameObject.Find("AudioManager");
        audioManager.GetComponent<AudioSource>().clip = mainMenuClip;
        audioManager.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(sceneIndex);
    }

    public void PilihCerita(string namaCerita)
    {
        levelSelector.chosenStory = namaCerita;
    }

    public void SelfDestruct(){
        Destroy(GameObject.FindGameObjectWithTag("LevelSelector"));
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
