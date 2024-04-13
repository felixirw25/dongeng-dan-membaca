using UnityEngine;
using Firebase;
using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
public class FirebaseInit : MonoBehaviour
{
    [SerializeField] private GameObject noSignal;
    void Start(){
        noSignal.SetActive(false);
        
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            FirebaseApp app = FirebaseApp.DefaultInstance;
        });
    }

    void Update(){
        if(Application.internetReachability == NetworkReachability.NotReachable){
            noSignal.SetActive(true);
        }
        else{
            noSignal.SetActive(false);
        }
    }
}
