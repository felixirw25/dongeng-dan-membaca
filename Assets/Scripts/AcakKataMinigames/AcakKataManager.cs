using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

public class AcakKataManager : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseFirestore firestore;
    FirebaseStorage storage;
    [SerializeField] GameObject questionPanelSusunKata;
    [SerializeField] private GameObject noSignal;
    [SerializeField] public GameObject cover;
    [SerializeField] AudioClip mainMenuClip;
    [SerializeField] public List<SceneQuiz> sceneQuizCollections;
    [SerializeField] private int indeks;
    [SerializeField] public int Indeks { get => indeks; set => indeks = value; }
    [SerializeField] public GameObject resultCanvas;
    [SerializeField] public GameObject completedPanel;
    [SerializeField] public GameObject failedPanel;
    [SerializeField] public GameObject kuisHabisPanel;
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
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI hintText;
    public float timer;
    public float waktu_kuis;
    public AcakKataGameController acakKataGameController;
    public AcakKata acakKata;
    public SceneQuiz currentQuiz;
    public QuizTypeAcakKata quizTypes;
    public bool Stop;
    private bool isAnimating = false;

    [Obsolete]
    private async void Awake(){
        auth = FirebaseAuth.DefaultInstance;
        firestore = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
        storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        sceneQuizCollections = new List<SceneQuiz>();

        await firestore.Collection("stories").GetSnapshotAsync().ContinueWithOnMainThread(async taskScene => {
            QuerySnapshot queryDocuments = taskScene.Result; 
            foreach(DocumentSnapshot storyDocument in queryDocuments.Documents){             
                var quizCollection = firestore.Collection("stories").Document(storyDocument.Id.ToString()).Collection("quizzes");
                var quizSnapshot = await quizCollection.WhereEqualTo("quizType", "Menyusun").GetSnapshotAsync();
                
                foreach(DocumentSnapshot documentQuiz in quizSnapshot.Documents){
                    SceneQuiz newSceneQuiz = new SceneQuiz();

                    Dictionary<string, object> quiz = documentQuiz.ToDictionary();
                    newSceneQuiz.Answer = (string)quiz["answer"];
                    newSceneQuiz.Hint = (string)quiz["hint"];
                    long idKuis = (long)quiz["id_kuis"];
                    newSceneQuiz.QuizId = (int)idKuis;
                    newSceneQuiz.PilihanPilgan = ((List<object>)quiz["pilihanPilgan"]).Cast<string>().ToList();
                    newSceneQuiz.Question = (string)quiz["question"];
                    newSceneQuiz.QuestionImageLink = (string)quiz["questionImage"];
                    newSceneQuiz.QuizType = (string)quiz["quizType"];
                    newSceneQuiz.Waktu = Convert.ToInt32(quiz["waktu"]);
                    newSceneQuiz.Waktu_Salah = Convert.ToInt32(quiz["waktu_salah"]);

                    sceneQuizCollections.Add(newSceneQuiz);
                }
            }

            // Shuffle the list to randomize the order
            sceneQuizCollections = sceneQuizCollections.OrderBy(q => UnityEngine.Random.value).ToList();

            StartCoroutine(PlayQuiz(Indeks));
        });
    }
    private void Start()
    {
        Indeks = 0;
        StartCoroutine(LoadCover(3f));
        resultCanvas.SetActive(false);
        completedPanel.SetActive(false);
        failedPanel.SetActive(false);
        kuisHabisPanel.SetActive(false);
        hintPanel.SetActive(false);
        questionImage.SetActive(false);
        questionPanelSusunKata.SetActive(false);
        minusImage.SetActive(false);
        wrongImage.SetActive(false);

        timer = Mathf.Clamp(60, 0, 60);
        Stop = true;
        Time.timeScale = 1;
        TimeManager();
        acakKataGameController.pauseRestart.onClick.RemoveAllListeners();
        acakKataGameController.pauseRestart.onClick.AddListener(()=>{
            if(currentQuiz.QuizType=="Menyusun"){
                for(int i = 0; i<acakKata.startSlot.childCount; i++){
                    Destroy(acakKata.startSlot.GetChild(i).gameObject);
                }
                for(int i = 0; i<acakKata.answerSlot.childCount; i++){
                    Destroy(acakKata.answerSlot.GetChild(i).gameObject);
                }
            }
        });
    }

    public void TimeManager(){
        if(timerText==null)
            return;

        timerText.text = "";
        timer=60;
        
        questionText.enabled = true;
        timerText.enabled = true;
    }

    private void Update(){
        if(Application.internetReachability == NetworkReachability.NotReachable){
            noSignal.SetActive(true);
        }
        else{
            noSignal.SetActive(false);
        }

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
            resultCanvas.SetActive(true);
            failedPanel.SetActive(true);
            if(currentQuiz.QuizType=="Menyusun"){
                for(int i = 0; i<acakKata.startSlot.childCount; i++){
                    Destroy(acakKata.startSlot.GetChild(i).gameObject);
                }
                for(int i = 0; i<acakKata.answerSlot.childCount; i++){
                    Destroy(acakKata.answerSlot.GetChild(i).gameObject);
                }
            }
        }
        else if (Stop){
            failedPanel.SetActive(false);
        }
    }

    public IEnumerator PlayQuiz(int index)
    {
        yield return new WaitForSecondsRealtime(1);
        acakKataGameController.quizCanvas.SetActive(true);
        questionImage.SetActive(true);
        questionPanelSusunKata.SetActive(true);
        Stop = false;
        Debug.Log("index:" + index);
        Debug.Log("sceneQuizCollections.Count:" + sceneQuizCollections.Count);
        if (index < sceneQuizCollections.Count) // Ensure index is within the range
        {
            currentQuiz = sceneQuizCollections[index];
            timer = currentQuiz.Waktu;
            questionText.text = currentQuiz.Question;
            hintText.text = currentQuiz.Hint;
            // tts.ReadText(questionText.text);

            if (currentQuiz.QuestionImageLink != null)
            {
                questionImage.SetActive(true);
                StartCoroutine(LoadQuizImageIE(currentQuiz.QuestionImageLink.ToString()));
            }
            else
            {
                questionImage.SetActive(false);
            }
            acakKataGameController.pauseRestart.onClick.RemoveAllListeners();
            acakKataGameController.pauseRestart.onClick.AddListener(() =>
            {
                if (currentQuiz.QuizType == "Menyusun")
                {
                    for (int i = 0; i < acakKata.startSlot.childCount; i++)
                    {
                        Destroy(acakKata.startSlot.GetChild(i).gameObject);
                    }
                    for (int i = 0; i < acakKata.answerSlot.childCount; i++)
                    {
                        Destroy(acakKata.answerSlot.GetChild(i).gameObject);
                    }
                }
                Restart();
            });
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() =>
            {
                Restart();
            });
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() =>
            {
                Indeks++;
                resultCanvas.SetActive(false);
                Debug.Log("Indeks: "+Indeks);
                StartCoroutine(LoadCover(1f));
                StartCoroutine(PlayQuiz(Indeks));
            });
            ChooseAnswer();
        }
        else
        {
            Stop=true;
            completedPanel.SetActive(false);
            failedPanel.SetActive(false);
            resultCanvas.SetActive(true);
            kuisHabisPanel.SetActive(true);
        }
    }

    // 2.b.ii Mengaktifkan canvas sesuai jenis quiz
    public void ChooseAnswer()
    {
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

    [Obsolete]
    IEnumerator LoadQuizImageIE(string mediaUrl){
        StartCoroutine(acakKataGameController.sceneLoader.LoadCover(2f));
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

    public void Restart(){
        Stop = true;
        Time.timeScale=1;
        acakKataGameController.QuizTime = false;
        resultCanvas.SetActive(false);
        failedPanel.SetActive(false);
        acakKataGameController.QuizTime = false;
        acakKataGameController.quizCanvas.SetActive(false);
        acakKataGameController.PausePanel.SetActive(false);
        StartCoroutine(LoadCover(1f));
        StartCoroutine(PlayQuiz(Indeks));
    }

    public IEnumerator LoadCover(float second){
        cover.SetActive(true); 
        yield return new WaitForSecondsRealtime(second); 
        cover.SetActive(false);
    }
}