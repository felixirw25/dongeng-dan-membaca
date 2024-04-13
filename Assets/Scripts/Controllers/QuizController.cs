using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class QuizController : MonoBehaviour
{
    [SerializeField] GameObject questionPanelPilgan;
    [SerializeField] GameObject questionPanelSusunKata;
    [SerializeField] GameObject questionPanelMengisi;
    [SerializeField] public GameObject resultCanvas;
    [SerializeField] public GameObject completedPanel;
    [SerializeField] public GameObject failedPanel;
    [SerializeField] public GameObject hintPanel;
    [SerializeField] public PlayerProfile playerProfile;
    [SerializeField] public GameObject oneStar; 
    [SerializeField] public GameObject twoStar;
    [SerializeField] public GameObject threeStar;
    [SerializeField] GameObject minusImage;
    [SerializeField] GameObject wrongImage;
    [SerializeField] Button nextButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button hintButton;
    [SerializeField] GameObject questionImage;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip wrongClip;
    [SerializeField] AudioClip correctClip;
    [SerializeField] AudioClip mainMenuClip;
    [SerializeField] GameObject kolomJawaban;
    // [SerializeField] TextToSpeech tts;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI hintText;
    public int quizIndex;
    public float timer;
    public float waktu_kuis;
    public GameController gameController;
    public KataManager kataManager;
    public SceneQuiz currentQuiz;
    public QuizTypes quizTypes;
    public bool Stop;
    private bool isAnimating = false;

    [Header("Inisiasi Firebase")]
    public LevelSelector levelSelector;
    public UserData userData;
    private string spacedAnswer = "";
    private void Start(){
        resultCanvas.SetActive(false);
        completedPanel.SetActive(false);
        failedPanel.SetActive(false);
        hintPanel.SetActive(false);
        questionImage.SetActive(false);
        questionPanelPilgan.SetActive(false);
        questionPanelSusunKata.SetActive(false);
        questionPanelMengisi.SetActive(false);
        minusImage.SetActive(false);
        wrongImage.SetActive(false);

        timer = Mathf.Clamp(60, 0, 60);
        Stop = true;
        Time.timeScale = 1;
        TimeManager();
        userData = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>();
        userData.ReadUserDataAsync();
        gameController.pauseRestart.onClick.RemoveAllListeners();
        gameController.pauseRestart.onClick.AddListener(()=>{
            if(currentQuiz.QuizType=="Menyusun"){
                for(int i = 0; i<kataManager.startSlot.childCount; i++){
                    Destroy(kataManager.startSlot.GetChild(i).gameObject);
                }
                for(int i = 0; i<kataManager.answerSlot.childCount; i++){
                    Destroy(kataManager.answerSlot.GetChild(i).gameObject);
                }
            }
            if(currentQuiz.QuizType=="Mengisi"){
                quizTypes.jawabanMengisi.GetComponent<Text>().text = "";
            }
        });
    }

    // Memulai countdown timer quiz
    private void Update()
    {
        // if(timerText==null || gameOverPanel==null)
        if(timerText==null)
            return;
        
        if(Stop==false){
            if(timer>0)
                timer -= Time.deltaTime;
            else
                timer = 0;
        }
        
        timerText.text = timer.ToString("0");
        if(timer==0){
            timer=-1;
            Stop=true;
            resultCanvas.SetActive(true);
            failedPanel.SetActive(true);
            if(currentQuiz.QuizType=="Menyusun"){
                for(int i = 0; i<kataManager.startSlot.childCount; i++){
                    Destroy(kataManager.startSlot.GetChild(i).gameObject);
                }
                for(int i = 0; i<kataManager.answerSlot.childCount; i++){
                    Destroy(kataManager.answerSlot.GetChild(i).gameObject);
                }
            }
            if(currentQuiz.QuizType=="Mengisi"){
                quizTypes.jawabanMengisi.GetComponent<Text>().text = "";
            }
        }
    }
    // Mengatur countdown timer quiz
    public void TimeManager(){
        if(timerText==null)
            return;

        timerText.text = "";
        timer=60;
        
        questionText.enabled = true;
        timerText.enabled = true;
    }

    // 2.b.i Menampilkan quiz dan mengatur button failed dan sukses
    [Obsolete]
    public void PlayQuiz(int index)
    {
        Stop = false;
        // quizIndex = index/3;
        levelSelector = GameObject.FindGameObjectWithTag("LevelSelector").GetComponent<LevelSelector>();
        quizIndex = index/levelSelector.variasiLevel;
        currentQuiz = (SceneQuiz)levelSelector.sortedAndRandomizedSceneQuizCollections[quizIndex][0];
        timer=currentQuiz.Waktu;

        if(currentQuiz.QuizType=="Mengisi"){
            Debug.Log("currentQuiz.Answer: " + currentQuiz.Answer);
            string questionAnswer = currentQuiz.Answer;
            string[] answerWords = questionAnswer.Split(' ');
            // Create a new string with underscores and spaces
            string underscoredAnswer = "";
            foreach (string word in answerWords)
            {
                underscoredAnswer += new string('_', word.Length) + " ";
            }
            spacedAnswer = "";
            for (int i = 0; i < underscoredAnswer.Length; i++)
            {
                spacedAnswer += underscoredAnswer[i] + " ";
            }

            // Remove the extra space at the end of spacedAnswer
            spacedAnswer = spacedAnswer.TrimEnd();
            Debug.Log("result: " + spacedAnswer);

            questionText.text = currentQuiz.Question + ":";
        }
        else{
            questionText.text = currentQuiz.Question;
        }

        hintText.text = currentQuiz.Hint;
        // tts.ReadText(questionText.text);

        if(currentQuiz.QuestionImageLink != null){
            questionImage.SetActive(true);
            StartCoroutine(LoadQuizImageIE(currentQuiz.QuestionImageLink.ToString()));
        }
        else{
            questionImage.SetActive(false);
        }
        gameController.pauseRestart.onClick.RemoveAllListeners();
        gameController.pauseRestart.onClick.AddListener(()=>{
            if(currentQuiz.QuizType=="Menyusun"){
                for(int i = 0; i<kataManager.startSlot.childCount; i++){
                    Destroy(kataManager.startSlot.GetChild(i).gameObject);
                }
                for(int i = 0; i<kataManager.answerSlot.childCount; i++){
                    Destroy(kataManager.answerSlot.GetChild(i).gameObject);
                }
            }
            // if(currentQuiz.QuizType=="Mengisi"){
            //     quizTypes.jawabanMengisi.GetComponent<Text>().text = "";
            // }
            Restart(index);
        });
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(()=>{
            Restart(index);
        });
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(()=>{
            resultCanvas.SetActive(false);
            gameController.quizCanvas.SetActive(false);
            failedPanel.SetActive(false);
            gameController.QuizTime = false;
        });
        ChooseAnswer();
    }

    // 2.b.ii Mengaktifkan canvas sesuai jenis quiz
    public void ChooseAnswer()
    {
        var questionType = currentQuiz.QuizType;
        if(questionType.ToString()=="Pilgan"){
            questionPanelPilgan.SetActive(true);
            questionPanelSusunKata.SetActive(false);
            questionPanelMengisi.SetActive(false);
        }
        else if(questionType.ToString()=="Menyusun"){
            questionPanelPilgan.SetActive(false);
            questionPanelSusunKata.SetActive(true);
            questionPanelMengisi.SetActive(false);
        }
        else{
            questionPanelPilgan.SetActive(false);
            questionPanelSusunKata.SetActive(false);
            questionPanelMengisi.SetActive(true);
            kolomJawaban.SetActive(true);
            kolomJawaban.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = spacedAnswer;
        }
        quizTypes.checkQuizType((SceneQuiz)currentQuiz);
    }

    public void Correct(){
        audioSource.PlayOneShot(correctClip);
    }

    public void Wrong(){
        if(isAnimating==false)
            StartCoroutine(WrongAnimationIE());
    }

    private IEnumerator WrongAnimationIE(){
        isAnimating=true;
        minusImage.SetActive(true);
        wrongImage.SetActive(true);
        audioSource.PlayOneShot(wrongClip);
        var minusTime = minusImage.GetComponent<Animator>();
        var wrong = wrongImage.GetComponent<Animator>();
        minusTime.Play("Base Layer.MinusTime");
        wrong.Play("Base Layer.Wrong");
        yield return new WaitForSecondsRealtime(1);
        minusImage.SetActive(false);
        wrongImage.SetActive(false);
        isAnimating=false;
    }

    // public void ExitQuiz(string sceneIndex){
    //     Destroy(GameObject.FindGameObjectWithTag("LevelSelector"));
    //     var audioManager = GameObject.Find("AudioManager");
    //     audioManager.GetComponent<AudioSource>().clip = mainMenuClip;
    //     audioManager.GetComponent<AudioSource>().Play();
    //     SceneManager.LoadScene(sceneIndex);
    // }

    [Obsolete]
    IEnumerator LoadQuizImageIE(string mediaUrl){
        StartCoroutine(gameController.sceneLoader.LoadCover(2f));
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest(); 
        if(request.isNetworkError || request.isHttpError){
            questionImage.SetActive(false);
        }
        else{
            questionImage.SetActive(true);
            questionImage.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    [Obsolete]
    public void Restart(int index){
        Stop = true;
        Time.timeScale=1;
        gameController.QuizTime = false;
        resultCanvas.SetActive(false);
        gameController.quizCanvas.SetActive(false);
        failedPanel.SetActive(false);
        gameController.QuizTime = false;
        gameController.PausePanel.SetActive(false);
        gameController.currentScene1 = gameController.levelSelector.sceneStoryCollections[index-levelSelector.variasiLevel];
        levelSelector.levelSelected = index-levelSelector.variasiLevel;
        gameController.sceneNumber.text = levelSelector.levelSelected+1 + " / " + levelSelector.sceneStoryCollections.Count;
        gameController.backgroundController.SwitchImage(gameController.currentScene1);
        gameController.bottomBar.PlayScene(gameController.currentScene1);
        Debug.Log("gameController.currentScene1.AudioLink:"+gameController.currentScene1.AudioLink);
        if(gameController.currentScene1.AudioLink != ""){
            Debug.Log("gameController.currentScene1.AudioLink:"+gameController.currentScene1.AudioLink);
            StartCoroutine(gameController.musicManager.LoadAudioIE(gameController.currentScene1.AudioLink));
        }
    }
}
