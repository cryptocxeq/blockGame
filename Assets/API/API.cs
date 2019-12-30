using System;
using Firebase;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Timeline;

public class API : MonoBehaviour
{
    DependencyStatus _dependencyStatus = DependencyStatus.UnavailableOther;

    public static API Instance;
    public AdmobManager Admob;
    public UnityAdsManager Unity;
    public ApiData Data;
    public static bool IsAdsClick;
    public float StartTime;

    void Awake()
    {
        StartTime = Time.time;
        Instance = this;
        Data.AndroidInfor.GameLink = "https://play.google.com/store/apps/details?id=" + Application.identifier;
    }

    void Start()
    {
        InitAds();
        InitFireBase();
    }

    void InitAds()
    {
        Admob = new AdmobManager();
        Unity = new UnityAdsManager();
        if (Application.platform == RuntimePlatform.Android)
        {
            Admob.Init(Data.AndroidInfor.Admob);
            Unity.Init(Data.AndroidInfor.UnityVideoId);

            Debug.Log("API init android infors");
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Admob.Init(Data.IosInfor.Admob);
            Unity.Init(Data.IosInfor.UnityVideoId);
            Debug.Log("API init iOS infors");
        }
        else
        {
            Admob.Init(Data.IosInfor.Admob);
            Unity.Init(Data.IosInfor.UnityVideoId);
            Debug.Log("API init other platform infors");
        }
    }

    void InitFireBase()
    {
        _dependencyStatus = FirebaseApp.CheckDependencies();
        if (_dependencyStatus != DependencyStatus.Available)
        {
            FirebaseApp.FixDependenciesAsync().ContinueWith(task =>
            {
                _dependencyStatus = FirebaseApp.CheckDependencies();
                if (_dependencyStatus == DependencyStatus.Available)
                    InitializeFirebase();
                else
                    Debug.LogError("Could not resolve all Firebase dependencies: " + _dependencyStatus);
            });
        }
        else
            InitializeFirebase();

        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
    }

    void InitializeFirebase()
    {
        Debug.Log("Enabling data collection.");
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        Debug.Log("Set user properties.");
        FirebaseAnalytics.SetUserProperty(FirebaseAnalytics.UserPropertySignUpMethod, "Google");
        FirebaseAnalytics.SetUserId("uber_user_510");
    }

    public void ShowBanner()
    {
        Admob.ShowBanner();
    }

    public void HideBanner()
    {
        Admob.HideBanner();
    }

    public void ShowFull(Action closedCallback)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            closedCallback();
            return;
        }

        Admob.ShowFull(closedCallback);
    }

    public void ShowVideo(Action rewardedCallback)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            rewardedCallback();
            return;
        }

        if (Admob.RewardBasedVideo.IsLoaded())
            Admob.ShowVideo(rewardedCallback);
        else
        {
            Admob.RequestVideo();
            Unity.ShowRewardedAd(rewardedCallback);
        }
    }

    public bool IsHasVideo()
    {
        if (Admob.RewardBasedVideo.IsLoaded())
            return true;
        if (Unity.IsVideoAvailable())
            return true;
        return false;
    }


    public void RateApp()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Application.OpenURL(Data.AndroidInfor.GameLink);
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Application.OpenURL(Data.IosInfor.GameLink);
        }
        else
        {
            Debug.Log("Button rate click");
        }

        FirebaseAnalytics.LogEvent("user_action", "click_button", "click_rate_button");
    }

    public void GotoStore()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Application.OpenURL(Data.AndroidInfor.StoreLink);
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Application.OpenURL(Data.IosInfor.StoreLink);
        }
        else
        {
            Debug.Log("Button moregame click");
        }

        FirebaseAnalytics.LogEvent("user_action", "click_button", "click_moregame_button");
    }


    private void OnApplicationPause(bool pauseStatus)
    {
        if (Time.time - StartTime < 20)
            return;
        if (!pauseStatus)
        {
            if (!IsAdsClick)
            {
                ShowFull(() => { StartTime = Time.time; });
            }

            IsAdsClick = false;
        }
    }

    //void CallBackVideo()
    //{
    //    if (Admob != null)
    //        if (Admob._onVideoRewarded != null)
    //            Admob._onVideoRewarded();
    //}


    public static bool IsAndroid()
    {
#if UNITY_ANDROID
        return true;
#endif
        return false;
    }

    public static bool IsIOS()
    {
#if UNITY_IOS
        return true;
#endif
        return false;
    }
}