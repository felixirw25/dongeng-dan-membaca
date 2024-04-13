// using System;
// using System.Collections;
// using System.Collections.Generic;
// using GoogleMobileAds.Api;
// using UnityEngine;

// public class IklanManager : MonoBehaviour
// {
//     public PlayerProfile playerProfile;
//     private RewardedAd rewardedAd;
//     public GameObject quizCanvas;
    
//     public void Start()
//     {
//         MobileAds.Initialize(initialize=>{});
//     }
//     public void RequestRewarded()
//     {
//         string adUnitId;
//         #if UNITY_ANDROID
//             adUnitId = "ca-app-pub-3479088186706392/4388496713";
//         #else
//             adUnitId = "unexpected_platform";
//         #endif

//         this.rewardedAd = new RewardedAd(adUnitId);

//         // Called when an ad request has successfully loaded.
//         this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
//         // Called when an ad request failed to load.
//         this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
//         // Called when an ad is shown.
//         this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
//         // Called when an ad request failed to show.
//         this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
//         // Called when the user should be rewarded for interacting with the ad.
//         this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
//         // Called when the ad is closed.
//         this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

//         // Create an empty ad request.
//         AdRequest request = new AdRequest.Builder().Build();
//         // Load the rewarded ad with the request.
//         this.rewardedAd.LoadAd(request);
//         if(this.rewardedAd.IsLoaded()){
//             Time.timeScale = 0;
//             quizCanvas.SetActive(false);
//             quizCanvas.SetActive(true);
//             rewardedAd.Show();
//         }
//     }

//     public void HandleRewardedAdLoaded(object sender, EventArgs args)
//     {
//         MonoBehaviour.print("HandleRewardedAdLoaded event received");
//     }

//     public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
//     {
//         Debug.Log("Ads Failed to Load");
//         Time.timeScale = 1;
//     }

//     public void HandleRewardedAdOpening(object sender, EventArgs args)
//     {
//         MonoBehaviour.print("HandleRewardedAdOpening event received");
//     }

//     public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
//     {
//         MonoBehaviour.print(
//             "HandleRewardedAdFailedToShow event received with message: "
//                              + args.Message);
//     }

//     public void HandleRewardedAdClosed(object sender, EventArgs args)
//     {
//         MonoBehaviour.print("HandleRewardedAdClosed event received");
//         quizCanvas.SetActive(false);
//         quizCanvas.SetActive(true);
//     }

//     public void HandleUserEarnedReward(object sender, Reward args)
//     {
//         Debug.Log("Reward Given");
//     }
// }

// // using System;
// // using System.Collections;
// // using System.Collections.Generic;
// // using GoogleMobileAds.Api;
// // using UnityEngine;

// // public class IklanManager : MonoBehaviour
// // {
// //     private RewardedAd Rewarded_Android;
// //     private string rewarded_Android_ID;
// //     void Start(){
// //         rewarded_Android_ID = "ca-app-pub-3940256099942544/5224354917";

// //         MobileAds.Initialize(initStatus=>{});
// //         RequestRewarded();
// //     }
// //     private void RequestRewarded(){
// //         Rewarded_Android = new RewardedAd(rewarded_Android_ID);
// //         Rewarded_Android.OnUserEarnedReward += HandleUserEarnedReward;
// //         Rewarded_Android.OnAdClosed += HandleRewardedAdClosed;
// //         Rewarded_Android.OnAdFailedToShow += HandleRewardedAdFailedToShow;
// //         AdRequest adRequest = new AdRequest.Builder().Build();
// //     }
// //     public void ShowRewarded(){
// //         if(Rewarded_Android.IsLoaded()){
// //             Rewarded_Android.Show();
// //             RequestRewarded();
// //         }
// //     }
// //     public void HandleRewardedAdLoaded(object sender, EventArgs args)
// //     {
// //         Debug.Log("HandleRewardedAdLoaded event received");
// //     }
// //     public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
// //     {
// //         Debug.Log("HandleRewardedAdFailedToLoad event received with message: ");
// //     }

// //     public void HandleRewardedAdOpening(object sender, EventArgs args)
// //     {
// //         Debug.Log("HandleRewardedAdOpening event received");
// //     }

// //     public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
// //     {
// //         Debug.Log("HandleRewardedAdFailedToShow event received with message: " + args.Message);
// //     }

// //     public void HandleRewardedAdClosed(object sender, EventArgs args)
// //     {
// //         Debug.Log("HandleRewardedAdClosed event received");
// //         Time.timeScale=1;
// //     }

// //     public void HandleUserEarnedReward(object sender, Reward args)
// //     {
// //         Debug.Log("HandleRewardedAdRewarded event received");
// //     }
// // }

// // using System;
// // using System.Collections;
// // using System.Collections.Generic;
// // using UnityEngine;
// // using UnityEngine.Advertisements;

// // public class IklanManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
// // {
// //     #if UNITY_ANDROID
// //         string gameId = "5037763";
// //         string Rewarded = "Rewarded_Android";
// //     #elif UNITY_IOS
// //         string gameId = "5037762";
// //         string Rewarded = "Rewarded_iOS";
// //     #endif

// //     private void Start(){
// //         Advertisement.Initialize(
// //             gameId, 
// //             false, 
// //             this);
// //     }

// //     public void ShowRewarded(){
// //         Advertisement.Load(placementId: Rewarded, loadListener: this);
// //         Advertisement.Show(placementId: Rewarded, showListener: this);
// //     }

// //     // Inizialization Callbacks
// //     public void OnInitializationComplete()
// //     {
// //         Debug.Log("OnInitializationComplete");
// //     }

// //     public void OnInitializationFailed(UnityAdsInitializationError error, string message)
// //     {
// //         Debug.Log("OnInitializationFailed: [ " + error + " ] " + message);
// //     }

// //     // Load Callbacks

// //     public void OnUnityAdsAdLoaded(string placementId)
// //     {
// //         Debug.Log("OnUnityAdsAdLoaded: [ " + placementId + " ]");
// //     }

// //     public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
// //     {
// //         Debug.Log("OnUnityAdsFailedToLoad: [ " + placementId + " ] [ " + error + " ] " + message);
// //     }

// //     // Show Callback
// //     public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
// //     {
// //         Debug.Log("OnUnityAdsShowFailure: [ " + placementId + " ] [ " + error + " ] " + message);
// //     }

// //     public void OnUnityAdsShowStart(string placementId)
// //     {
// //         Debug.Log("OnUnityAdsShowStart: [ " + placementId + " ]");
// //     }

// //     public void OnUnityAdsShowClick(string placementId)
// //     {
// //         Debug.Log("OnUnityAdsShowClick: [ " + placementId + " ]");
// //     }

// //     public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
// //     {
// //         Debug.Log("OnUnityAdsShowComplete: [ " + placementId + " ], " + "showCompletionState: " + showCompletionState);
// //         Time.timeScale=1;
// //     }

// //     // Ads Callback
// //     public void OnUnityAdsReady(string placementId)
// //     {
// //         Debug.Log("OnUnityAdsShowFailure: [ " + placementId + " ]");
// //     }

// //     public void OnUnityAdsDidError(string message)
// //     {
// //         Debug.Log("OnUnityAdsShowStart: [ " + message + " ]");
// //     }

// //     public void OnUnityAdsDidStart(string placementId)
// //     {
// //         Debug.Log("OnUnityAdsShowClick: [ " + placementId + " ]");
// //     }

// //     public void OnUnityAdsDidFinish(string placementId, UnityAdsShowCompletionState showResult)
// //     {
// //         Debug.Log("OnUnityAdsShowComplete: [ " + placementId + " ], " + showResult);
// //     }

// //     public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
// //     {
// //         Debug.Log("OnUnityAdsShowComplete: [ " + placementId + " ], " + showResult);
// //     }
// // }