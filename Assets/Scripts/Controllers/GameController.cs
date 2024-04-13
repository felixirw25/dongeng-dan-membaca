using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine.Networking;

public class GameController : MonoBehaviour
{
    public BottomBarController bottomBar;
    public BackgroundController backgroundController;
    public GameObject storyCanvas;
    public SceneStory currentScene1;
    public QuizController currentQuiz;
    public GameObject quizTimePanel;
    public Button quizTimeButton;
    public GameObject quizCanvas;
    private bool quizTime = false;
    public bool QuizTime { get => quizTime; set => quizTime = value; }
    public GameObject ceritaSelesaiPanel;
    public GameObject PausePanel;
    public Button PauseButton;
    public Button pauseRestart;
    public Button closeHintButton;
    public GameObject hintPanel;
    public Button HintButton;
    public PlayerProfile playerProfile;
    public bool GamePause;
    public LevelSelector levelSelector;
    public GameObject audioManager;
    public MusicManager musicManager;
    public SceneLoader sceneLoader;
    public UserData userData;
    bool update;
    // public IklanManager iklanManager;
    public FirebaseAuth auth;
    public string CharacterPath;
    [FirestoreData]
    public struct CharacterData{
        [FirestoreProperty]
        public int Star { get; set; }
        [FirestoreProperty]
        public int Hint { get; set; }
    }
    public Text sceneNumber;

    // 1. Memutar scene
    [Obsolete]
    void Start()
    {
        StopAllCoroutines();
        QuizTime = false;
        update = false;
        storyCanvas.SetActive(true);
        quizTimePanel.SetActive(false);
        quizCanvas.SetActive(false);
        hintPanel.SetActive(false);
        PausePanel.SetActive(false);
        ceritaSelesaiPanel.SetActive(false);
        StartCoroutine(sceneLoader.LoadCover(1f));
        levelSelector = GameObject.FindGameObjectWithTag("LevelSelector").GetComponent<LevelSelector>();
        currentScene1 = levelSelector.sceneStoryCollections[levelSelector.levelSelected];
        bottomBar.PlayScene(currentScene1);
        backgroundController.SwitchImage(currentScene1);
        sceneNumber.text = levelSelector.levelSelected+1 + " / " + levelSelector.sceneStoryCollections.Count;
        audioManager = GameObject.FindGameObjectWithTag("MusicManager");
        musicManager = audioManager.GetComponent<MusicManager>();
        if(currentScene1.MusicLink != ""){
            musicManager.LoadMusic(currentScene1);
        }
        if(currentScene1.AudioLink != ""){
            Debug.Log("currentScene1.AudioLink:"+currentScene1.AudioLink);
            StartCoroutine(musicManager.LoadAudioIE(currentScene1.AudioLink));
        }

        List<int> listCerita = new List<int>();
        playerProfile.thePlayer = new PlayerProfile(3);

        HintButton.onClick.AddListener(()=>
        {
            Hint();
        });

        closeHintButton.onClick.AddListener(()=>
        {
            closeHint();
        });
    }

    // 2. Menampilkan scene berikut & memeriksa apakah sudah 3 scene, maka Quiz Time
    [Obsolete]
    void Update()
    {
        if(QuizTime==false && bottomBar.IsCompleted())
        {
            if(levelSelector.levelSelected+1 >= levelSelector.sceneStoryCollections.Count){
                if(update==false){
                    update=true;
                    userData = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>();
                    ceritaSelesaiPanel.SetActive(true);
                    userData.newPlayerProfile.isFinishedList[levelSelector.ceritaTerpilih.Id_cerita-1]="true";
                    userData.WriteUserData();
                    return;
                }
            }
            else{
                currentScene1 = levelSelector.sceneStoryCollections[levelSelector.levelSelected+=1];
                bottomBar.PlayScene(currentScene1);
                backgroundController.SwitchImage(currentScene1);
                sceneNumber.text = levelSelector.levelSelected+1 + " / " + levelSelector.sceneStoryCollections.Count;
                if(currentScene1.MusicLink != ""){
                    musicManager.LoadMusic(currentScene1);
                }
                if(currentScene1.AudioLink != ""){
                    Debug.Log("currentScene1.AudioLink:"+currentScene1.AudioLink);
                    StartCoroutine(musicManager.LoadAudioIE(currentScene1.AudioLink));
                }
                
                // if((levelSelector.levelSelected!=-1) && ((levelSelector.levelSelected+1)%3==0)){
                if((levelSelector.levelSelected!=-1) && ((levelSelector.levelSelected+1)%levelSelector.variasiLevel==0)){
                    // if (levelSelector.sortedAndRandomizedSceneQuizCollections[Mathf.FloorToInt(levelSelector.levelSelected/3)+1][0].Question != "")
                    List<SceneQuiz> value;
                    if (levelSelector.sortedAndRandomizedSceneQuizCollections.TryGetValue(Mathf.FloorToInt(levelSelector.levelSelected/levelSelector.variasiLevel)+1, out value))
                    {
                        if (levelSelector.sortedAndRandomizedSceneQuizCollections[Mathf.FloorToInt(levelSelector.levelSelected/levelSelector.variasiLevel)+1][0].Question != "")
                        {
                            QuizTime = true;
                            StartCoroutine(ShowQuizTime());
                            quizTimeButton.onClick.RemoveAllListeners();
                            quizTimeButton.onClick.AddListener(()=>{
                                quizTimePanel.SetActive(false);
                                currentQuiz.timer = Mathf.Clamp(60, 0, 60);
                                ShowQuiz(levelSelector.levelSelected+1);
                            });
                        }
                    }
                }
            }
        }
    }

    // 2.a Menunggu scene kelipatan 3 selesai ditampilkan dan klik lanjut dari bottombar.IsCompleted()
    private IEnumerator ShowQuizTime(){
        yield return new WaitUntil(()=>bottomBar.IsCompleted());
        int error = 0;
        try
        {
            var cekSceneSekarang = levelSelector.sceneStoryCollections.IndexOf(levelSelector.sceneStoryCollections[levelSelector.levelSelected], levelSelector.levelSelected)+1;   
        }
        catch (System.Exception)
        {
            error = 1;
            QuizTime = false;
        }
        if(error==0){
            quizTimePanel.SetActive(true);
        }
    }

    // 2.b Menampilkan quiz
    [Obsolete]
    private void ShowQuiz(int cekSceneSekarang)
    {
        quizCanvas.SetActive(true);
        currentQuiz.PlayQuiz(cekSceneSekarang);
    }
    // 3. Pausecontrol
    public void PauseControl()
    {
        if (Time.timeScale == 1)
        {
            PausePanel.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            PausePanel.SetActive(false);
        }
    }

    // hint
    public void Hint()
    {
        // if (playerProfile.thePlayer.HintCount > 0)
        // {
            hintPanel.SetActive(true);
            Time.timeScale = 0;
            // playerProfile.thePlayer.HintCount -= 1;
            // Debug.Log("jumlah hint sekarang: " + playerProfile.thePlayer.HintCount);
        // }
        // else
        // {
            // Debug.Log("hint habis");
            // Time.timeScale = 1;
            // iklanManager.RequestRewarded();
            // playerProfile.thePlayer.HintCount += 1;
            // Debug.Log("jumlah hint sekarang: " + playerProfile.thePlayer.HintCount);
        // }
    }

    public void closeHint()
    {
        hintPanel.SetActive(false);
        Time.timeScale = 1;
    }
}