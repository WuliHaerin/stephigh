﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;
#if AdmobDef
using GoogleMobileAds;
using GoogleMobileAds.Api;
#endif

/// <summary>
/// Script that handle your banner , noraml and reward ads
/// </summary>

// Example script showing how to invoke the Google Mobile Ads Unity plugin.
public class AdmobAdsScript : MonoBehaviour
{
    public static AdmobAdsScript instance;

    public managerVars vars;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    //public string appId, bannerId, videoId;

    public bool isTesting = true;
    [Header("Only for Android Devices")]
    [Header("Dont Know ID try \"Device ID Finder for AdMob\" App")]
    public string Device_ID = "586DB8B2E6337BBD8C7565D71828E4BD";

    //...........................................Uncomment this line after importing google admob sdk

#if AdmobDef
    private BannerView bannerView;
    private InterstitialAd interstitial;
#endif

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
#if AdmobDef
        RequestBanner();
        RequestInterstitial();
#endif
    }

    void Awake()
    {
        MakeSingleton();
    }

    void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    void Start()
    {
#if AdmobDef
        MobileAds.Initialize(vars.admobAppID);
#endif
    }

#if AdmobDef
    // the following method is used when we are testing the ads
    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice(Device_ID)
            .AddKeyword("game")
            .SetGender(Gender.Male)
            .SetBirthday(new DateTime(1993, 5, 27))
            .TagForChildDirectedTreatment(false)
            .AddExtra("color_bg", "9B30FF")
            .Build();
    }
#endif

    //.............................................................Methods used to request for ads
    //we use this methode to get the banner ads
#if AdmobDef
    private void RequestBanner()
    {
        // Clean up banner ad before creating a new one.
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }

#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
            string adUnitId = vars.adMobBannerID;
#elif UNITY_IOS
            string adUnitId = vars.adMobBannerID;
#else
            string adUnitId = "unexpected_platform";
#endif

        if (vars.bannerAdPoisiton == 0)
        {
            // Create a 320x50 banner at the top of the screen.
            bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
        }
        else if (vars.bannerAdPoisiton == 1)
        {
            // Create a 320x50 banner at the top of the screen.
            bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.BottomLeft);
        }
        else if (vars.bannerAdPoisiton == 2)
        {
            // Create a 320x50 banner at the top of the screen.
            bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.BottomRight);
        }
        else if (vars.bannerAdPoisiton == 3)
        {
            // Create a 320x50 banner at the top of the screen.
            bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Top);
        }
        else if (vars.bannerAdPoisiton == 4)
        {
            // Create a 320x50 banner at the top of the screen.
            bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.TopLeft);
        }
        else if (vars.bannerAdPoisiton == 5)
        {
            // Create a 320x50 banner at the top of the screen.
            bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.TopRight);
        }
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load a banner ad.
        //replace createAdRequest with request when the games is submitting to store
        if (isTesting)
        {
            bannerView.LoadAd(CreateAdRequest());
        }
        else if (!isTesting)
        {
            bannerView.LoadAd(request);
        }
    }

    //we use this methode to get the Interstitial ads
    private void RequestInterstitial()
    {
        // Clean up interstitial ad before creating a new one.
        if (this.interstitial != null)
        {
            this.interstitial.Destroy();
        }

#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
            string adUnitId = vars.adMobInterstitialID;
#elif UNITY_IOS
            string adUnitId = vars.adMobInterstitialID;
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create an interstitial.
        interstitial = new InterstitialAd(adUnitId);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load an interstitial ad.
        //replace createAdRequest with request when the games is submitting to store
        if (isTesting)
        {
            interstitial.LoadAd(CreateAdRequest());
        }
        else if (!isTesting)
        {
            interstitial.LoadAd(request);
        }

    }

    //.............................................................Methods used to show for ads
    //use this methode to show ads
    public void ShowInterstitial()
    {

#if UNITY_EDITOR
        Debug.Log("Interstitial Working");
#elif UNITY_ANDROID || UNITY_IOS
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
        }
        else
        {
            MonoBehaviour.print("Interstitial is not ready yet");
        }
#endif

    }

    //this methode is used to call the banner ads
    public void ShowBannerAds()
    {
        bannerView.Show();
    }

    //this methode is used to hide banner ads
    public void HideBannerAds()
    {
        bannerView.Hide();
    }

    //this methode is used to destroy banner ads
    public void DestroyBannerAds()
    {
        bannerView.Destroy();
    }
#endif
}