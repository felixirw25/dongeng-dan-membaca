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
using System.Threading.Tasks;
using System.Linq;

public class UserData : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseFirestore firestore;
    QuerySnapshot queryDocuments;
    [SerializeField] List<Cerita> listCeritaInfo;
    public PlayerProfile newPlayerProfile;
    public FirebaseAuthManager firebaseAuthManager;

    private void Awake(){
        auth = FirebaseAuth.DefaultInstance;
        firestore = FirebaseFirestore.DefaultInstance;
    }

    private void Start(){
        DontDestroyOnLoad(this);
        listCeritaInfo = new List<Cerita>();

        firestore.Collection("stories").OrderBy("id_cerita").GetSnapshotAsync().ContinueWithOnMainThread(task=>{
            QuerySnapshot queryDocuments = task.Result;

            foreach(DocumentSnapshot document in queryDocuments.Documents){
                Dictionary<string, object> cerita = document.ToDictionary();
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
        listCeritaInfo.OrderBy(c=>c.Id_cerita);
    }

    public async void ReadUserDataAsync(){
        await firestore.Collection("usersData").Document(auth.CurrentUser.UserId).Collection("progressCerita").OrderBy("id_cerita").GetSnapshotAsync().ContinueWithOnMainThread(task=>{
            queryDocuments = task.Result;
            newPlayerProfile = new PlayerProfile(listCeritaInfo.Count);

            foreach(DocumentSnapshot document in queryDocuments.Documents){
                Dictionary<string, object> progressCerita = document.ToDictionary();
                PlayerData playerDataBaru = new PlayerData();

                long idCerita = (long)progressCerita["id_cerita"];
                playerDataBaru.starPerLevelCerita = new List<List<int>>();
                
                playerDataBaru.idCerita = (int)idCerita;
                playerDataBaru.isFinished = (string)progressCerita["isFinished"];

                List<int> newStarPerLevel = new List<int>();
                foreach(string level in (List<object>)progressCerita["starPerLevel"]){
                    newStarPerLevel.Add(int.Parse(level));
                }
                newPlayerProfile.starPerLevelCerita[(int)idCerita-1] = newStarPerLevel;
                newPlayerProfile.isFinishedList[(int)idCerita-1 ] = (string)progressCerita["isFinished"];
            }
        });

        await firestore.Collection("usersData").Document(auth.CurrentUser.UserId).GetSnapshotAsync().ContinueWithOnMainThread(task=>{
            Dictionary<string, object> newPlayerData = task.Result.ToDictionary();
            newPlayerProfile.HintCount= Convert.ToInt32(newPlayerData["Hint"]);
            newPlayerProfile.StarSpentCount= Convert.ToInt32(newPlayerData["StarSpent"]);
        });
    }

    public async void WriteUserData(){
        Dictionary<string, object> temp = new Dictionary<string, object>();
        temp.Add("Star", newPlayerProfile.CountStarAll());
        await firestore.Collection("usersData").Document(auth.CurrentUser.UserId).UpdateAsync(temp).ContinueWithOnMainThread(totalStarWrite=>{});
        Debug.Log("New Player Profile Null: " + newPlayerProfile);
        for (int i = 1; i <= newPlayerProfile.starPerLevelCerita.Count; i++){
            await firestore.Collection("usersData").Document(auth.CurrentUser.UserId).Collection("progressCerita").WhereEqualTo("id_cerita", i).GetSnapshotAsync().ContinueWithOnMainThread(async taskRead =>{
                string docId = "";
                foreach (DocumentSnapshot document in taskRead.Result.Documents){
                    Dictionary<string, object> progressBintang = document.ToDictionary();
                    long idCerita = (long)progressBintang["id_cerita"];
                    if((int)idCerita==i){
                        docId = document.Id;
                    }
                }
                Debug.Log("newPlayerProfile.starPerLevelCerita[0].Count: " + newPlayerProfile.starPerLevelCerita[0].Count);
            await firestore.Collection("usersData").Document(auth.CurrentUser.UserId).Collection("progressCerita").Document(docId).UpdateAsync(await newPlayerProfile.ExportDataAsync(i, i, docId, 1)).ContinueWithOnMainThread(taskWrite=>{});
            });
        }
    }

    public async void CreateUserData(){
        newPlayerProfile = new PlayerProfile(listCeritaInfo.Count);
        Dictionary<string, object> tempHintStar = new Dictionary<string, object>();
        tempHintStar.Add("Hint", 3);
        tempHintStar.Add("Star", 0);
        tempHintStar.Add("StarSpent", 0);

        await firestore.Collection("accessories").GetSnapshotAsync().ContinueWithOnMainThread(taskReadAccesories=>{
            for (int i = 0; i < taskReadAccesories.Result.Count; i++){
                    Dictionary<string, object> progressAccessories = taskReadAccesories.Result[i].ToDictionary();
                    NewDataAccessories(taskReadAccesories.Result[i].Id.ToString().Trim());
                }
        });

        await firestore.Collection("usersData").Document(auth.CurrentUser.UserId).SetAsync(tempHintStar).ContinueWithOnMainThread(async createUserWrite=>{
            await firestore.Collection("stories").OrderBy("id_cerita").GetSnapshotAsync().ContinueWithOnMainThread(taskReadStories =>{
                for (int i = 0; i < taskReadStories.Result.Count; i++){
                    Dictionary<string, object> progressBintang = taskReadStories.Result[i].ToDictionary();
                    long idCerita = (long)progressBintang["id_cerita"];
                    long variasiAdegan = (long)progressBintang["variasi_adegan"];
                    NewData(taskReadStories.Result[i].Id.ToString().Trim(), (int)idCerita, taskReadStories.Result.Count-1, (int)variasiAdegan);
                }
            });
        });
    }

    public async void NewData(string id_dokumen, int i, int total, int variasiAdegan){
        Debug.Log("i adalah: " + i);
        Debug.Log("total adalah: " + total);
        Dictionary<string, object> createData = await newPlayerProfile.ExportDataAsync(i, i, id_dokumen, variasiAdegan);
        await firestore.Collection("usersData").Document(auth.CurrentUser.UserId).Collection("progressCerita").Document(id_dokumen).SetAsync(createData).ContinueWithOnMainThread(result=>{
            if (result.IsCompletedSuccessfully)
            {
                if(i==total){
                    // SceneManager.LoadScene("MainMenu");
                    StartCoroutine(firebaseAuthManager.HandleLoginSuccess());
                }
                Debug.Log("Create Account Data Succesfully!");
            }
            else{
                Debug.Log("Create Account Data Failed!");
            }
        });
    }
    public async void NewDataAccessories(string id_accessories){
        Dictionary<string, object> createDataAccessories = new Dictionary<string, object>();
        createDataAccessories.Add("data", "/accessories/"+id_accessories);
        createDataAccessories.Add("isUnlocked", "false");
        await firestore.Collection("usersData").Document(auth.CurrentUser.UserId).Collection("progressAccessories").Document(id_accessories).SetAsync(createDataAccessories);
    }
}
