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

public class GameResource : MonoBehaviour
{
    public List<Cerita> listCeritaInfo;
    FirebaseAuth auth;
    FirebaseFirestore firestore;
    [Header("Game Version")]
    [SerializeField] public GameObject versionCover;
    string gameVersion = "1.1";
    void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        firestore = FirebaseFirestore.DefaultInstance;
        listCeritaInfo = new List<Cerita>();

        firestore.Collection("stories").OrderBy("id_cerita").GetSnapshotAsync().ContinueWithOnMainThread(task=>{
            QuerySnapshot queryDocuments = task.Result;
            queryDocuments.Documents.OrderBy(c=>c.ToDictionary()["id_cerita"]);

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

    void Start()
    {
        versionCover.SetActive(false);
        DontDestroyOnLoad(this);
        FetchAndCheckVersion();
    }
    void FetchAndCheckVersion()
    {
        firestore.Collection("version").Document("lle6usWZiQH859aLkjM9").GetSnapshotAsync().ContinueWithOnMainThread(taskVersion=>{
            Dictionary<string, object> versionInfo = taskVersion.Result.ToDictionary();
            string fetchedVersion = (string)versionInfo["current"];
            
            if (fetchedVersion != gameVersion)
            {
                versionCover.SetActive(true);
            }
        });
    }
}
