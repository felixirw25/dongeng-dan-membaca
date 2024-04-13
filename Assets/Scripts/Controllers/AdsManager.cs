
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using GoogleMobileAds;
// using GoogleMobileAds.Api;
// using System;

// public class AdsManager : MonoBehaviour
// {
//     private string _adUnitId = "ca-app-pub-3940256099942544/6300978111";

//     BannerView _bannerView;


//     // Start is called before the first frame update
//     void Start()
//     {
//         MobileAds.Initialize((InitializationStatus initStatus) =>
//         {
//             CreateBannerView();
//             LoadAd();
//         });
//     }


//     public void CreateBannerView()
//     {
//         Debug.Log("Creating banner view");

//         // If we already have a banner, destroy the old one.
//         if (_bannerView != null)
//         {
//             DestroyAd();
//         }

//         // Create a 320x50 banner at top of the screen
//         _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Top);
//         ListenToAdEvents();
//     }

//     public void LoadAd()
//     {
//         // create an instance of a banner view first.
//         if (_bannerView == null)
//         {
//             CreateBannerView();
//         }
//         // create our request used to load the ad.
//         var adRequest = new AdRequest.Builder()
//             .AddKeyword("unity-admob-sample")
//             .Build();

//         // send the request to load the ad.
//         Debug.Log("Loading banner ad.");
//         _bannerView.LoadAd(adRequest);
//     }

//     private void ListenToAdEvents()
//     {
//         // Raised when an ad is loaded into the banner view.
//         _bannerView.OnBannerAdLoaded += () =>
//         {
//             Debug.Log("Banner view loaded an ad with response : "
//                 + _bannerView.GetResponseInfo());
//         };
//         // Raised when an ad fails to load into the banner view.
//         _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
//         {
//             Debug.LogError("Banner view failed to load an ad with error : "
//                 + error);
//         };
//         // Raised when the ad is estimated to have earned money.
//         _bannerView.OnAdPaid += (AdValue adValue) =>
//         {
//             Debug.Log(String.Format("Banner view paid {0} {1}.",
//                 adValue.Value,
//                 adValue.CurrencyCode));
//         };
//         // Raised when an impression is recorded for an ad.
//         _bannerView.OnAdImpressionRecorded += () =>
//         {
//             Debug.Log("Banner view recorded an impression.");
//         };
//         // Raised when a click is recorded for an ad.
//         _bannerView.OnAdClicked += () =>
//         {
//             Debug.Log("Banner view was clicked.");
//         };
//         // Raised when an ad opened full screen content.
//         _bannerView.OnAdFullScreenContentOpened += () =>
//         {
//             Debug.Log("Banner view full screen content opened.");
//         };
//         // Raised when the ad closed full screen content.
//         _bannerView.OnAdFullScreenContentClosed += () =>
//         {
//             Debug.Log("Banner view full screen content closed.");
//         };
//     }

//     public void DestroyAd()
//     {
//         if (_bannerView != null)
//         {
//             Debug.Log("Destroying banner ad.");
//             _bannerView.Destroy();
//             _bannerView = null;
//         }
//     }
// }