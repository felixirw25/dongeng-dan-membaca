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

public class LevelSelector : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseFirestore firestore;
    FirebaseStorage storage;
    public QuerySnapshot queryDocuments;
    public int levelSelected;
    public int variasiLevel;
    public string chosenStory;
    public Cerita ceritaTerpilih;
    [SerializeField] public List<SceneStory> sceneStoryCollections;
    [SerializeField] public List<SceneQuiz> sceneQuizCollections;
    [SerializeField] public Dictionary<int, List<SceneQuiz>> sortedAndRandomizedSceneQuizCollections;

    [Obsolete]
    private void Awake(){
        auth = FirebaseAuth.DefaultInstance;
        firestore = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
        storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        sceneStoryCollections = new List<SceneStory>();
        sceneQuizCollections = new List<SceneQuiz>();
        sortedAndRandomizedSceneQuizCollections = new Dictionary<int, List<SceneQuiz>>();
    }

    void Start(){
        levelSelected = 0;
    }

    void Update(){
        DontDestroyOnLoad(this.gameObject);
    }

    public void GetDataCerita(){
        // firestore.Collection("stories").Document(ceritaTerpilih.Id_dokumen).GetSnapshotAsync().ContinueWithOnMainThread(taskSceneVariant=>{
            // DocumentSnapshot documentSnapshot = taskSceneVariant.Result;
            // Dictionary<string, object> variasiScene = documentSnapshot.ToDictionary();
            // long variasiAdegan = (long)variasiScene["variasi_adegan"];
            // variasiLevel = (int)variasiAdegan;
            variasiLevel = ceritaTerpilih.Variasi_adegan;

            firestore.Collection("stories").Document(ceritaTerpilih.Id_dokumen).Collection("scenes").OrderBy("id_adegan").GetSnapshotAsync().ContinueWithOnMainThread(taskScene=>{
                queryDocuments = taskScene.Result;
                queryDocuments.Documents.OrderBy(c=>c.ToDictionary()["id_adegan"]);

                foreach(DocumentSnapshot documentStory in queryDocuments.Documents){
                    SceneStory newSceneStory = new SceneStory();

                    Dictionary<string, object> scene = documentStory.ToDictionary();
                    long idAdegan = (long)scene["id_adegan"];
                    newSceneStory.Id_adegan = (int)idAdegan;
                    newSceneStory.BackgroundLink = (string)scene["background"];
                    newSceneStory.AudioLink = (string)scene["audio"];
                    newSceneStory.MusicLink = (string)scene["music"];
                    newSceneStory.Text = (string)scene["storyText"];

                    sceneStoryCollections.Add(newSceneStory);
                }
                
                firestore.Collection("stories").Document(ceritaTerpilih.Id_dokumen).Collection("quizzes").GetSnapshotAsync().ContinueWithOnMainThread(taskQuiz=>{
                    queryDocuments = taskQuiz.Result;
                    
                    foreach(DocumentSnapshot documentQuiz in queryDocuments.Documents){
                        SceneQuiz newSceneQuiz = new SceneQuiz();

                        Dictionary<string, object> quiz = documentQuiz.ToDictionary();
                        newSceneQuiz.Answer = (string)quiz["answer"];
                        newSceneQuiz.Hint = (string)quiz["hint"];
                        long idKuis = (long)quiz["id_kuis"];
                        newSceneQuiz.QuizId = (int)idKuis;
                        newSceneQuiz.PilihanPilgan = new List<string>();
                        foreach(string pilihanPilgan in (List<object>)quiz["pilihanPilgan"]){
                            newSceneQuiz.PilihanPilgan.Add(pilihanPilgan);
                        }
                        newSceneQuiz.Question = (string)quiz["question"];
                        newSceneQuiz.QuestionImageLink = (string)quiz["questionImage"];
                        newSceneQuiz.QuizType = (string)quiz["quizType"];
                        long waktu = (long)quiz["waktu"];
                        newSceneQuiz.Waktu = (int)waktu;
                        long waktu_salah = (long)quiz["waktu_salah"];
                        newSceneQuiz.Waktu_Salah = (int)waktu_salah;

                        sceneQuizCollections.Add(newSceneQuiz);
                    }

                    // Group quizzes by QuizType
                    var groupedQuizzes = sceneQuizCollections.GroupBy(q => q.QuizId);

                    foreach (var group in groupedQuizzes)
                    {
                        // Randomize the quiz order within each group
                        List<SceneQuiz> randomizedList = group.OrderBy(q => UnityEngine.Random.value).ToList();
                        
                        // Store the randomized list in the dictionary
                        sortedAndRandomizedSceneQuizCollections[group.Key] = randomizedList;
                    }
                    
                    SceneManager.LoadScene("0_Level Selection");
                });
            });
        // });
    }
}