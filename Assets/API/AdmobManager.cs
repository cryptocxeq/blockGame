using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Firebase.Analytics;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdmobManager
{
    private Action _onFullClosed, _onVideoRewarded;

    public BannerView BannerView;
    public InterstitialAd Interstitial;
    public RewardBasedVideoAd RewardBasedVideo;
    private AdmobInfor _infor;

    public bool IsFullRequesting;
    public bool IsBannerRequesting;
    public bool IsVideoRequesting;

    private bool _isRewarded;

    public void Init(AdmobInfor infor)
    {
        _infor = infor;

        if (infor.IsUseBanner)
            RequestBanner();

        if (infor.IsUseFull)
            RequestFull();

        if (infor.IsUseVideo)
        {
            RewardBasedVideo = RewardBasedVideoAd.Instance;
            RewardBasedVideo.OnAdFailedToLoad += (sender, args) =>
            {
                IsVideoRequesting = false;
                Debug.Log("Video Failed " + args.Message);
                FirebaseAnalytics.LogEvent("Ads", "Video", "Failed");
            };
            RewardBasedVideo.OnAdClosed += (sender, args) =>
            {
                DOVirtual.DelayedCall(.01f, () =>
                {
                    if (_isRewarded)
                    {
                        if (_onVideoRewarded != null)
                            _onVideoRewarded();
                        _isRewarded = false;

                        Debug.Log("Video Closed");
                        FirebaseAnalytics.LogEvent("Ads", "Video", "Closed");

                        RequestVideo();
                    }
                });
            };
            RewardBasedVideo.OnAdLeavingApplication += (sender, args) =>
            {
                Debug.Log("Video Clicked");
                FirebaseAnalytics.LogEvent("Ads", "Video", "Clicked");
                API.IsAdsClick = true;
            };
            RewardBasedVideo.OnAdLoaded += (sender, args) =>
            {
                IsVideoRequesting = false;
                Debug.Log("Video Loaded");
                FirebaseAnalytics.LogEvent("Ads", "Video", "Loaded");
            };
            RewardBasedVideo.OnAdRewarded += (sender, reward) =>
            {
                _isRewarded = true;
                Debug.Log("Video Rewarded");
                FirebaseAnalytics.LogEvent("Ads", "Video", "Rewarded ");
            };

            RequestVideo();
        }
    }

    public void RequestBanner()
    {
        BannerView = new BannerView(_infor.AdmobBannerId, _infor.BannerSize, _infor.BannerPosition);
        FirebaseAnalytics.LogEvent("Ads", "Banner", "Request");
        BannerView.OnAdLeavingApplication += (sender, args) =>
        {
            Debug.Log("Banner Clicked");
            FirebaseAnalytics.LogEvent("Ads", "Banner", "Clicked");
            API.IsAdsClick = true;
        };
        BannerView.OnAdLoaded += (sender, args) =>
        {
            Debug.Log("Banner Loaded");
            FirebaseAnalytics.LogEvent("Ads", "Banner", "Loaded");
        };
        BannerView.OnAdFailedToLoad += (sender, args) =>
        {
            Debug.Log("Banner Failed " + args.Message);
            FirebaseAnalytics.LogEvent("Ads", "Banner", "Failed");
            BannerView = null;
        };
        BannerView.LoadAd(GetAdRequest());
    }

    public void RequestFull()
    {
        if (IsFullRequesting)
            return;
        IsFullRequesting = true;
        if (Interstitial != null)
            Interstitial.Destroy();

        FirebaseAnalytics.LogEvent("Ads", "Full", "Request");
        Interstitial = new InterstitialAd(_infor.AdmobFullId);
        Interstitial.OnAdLoaded += (sender, args) =>
        {
            IsFullRequesting = false;
            Debug.Log("Full Loaded");
            FirebaseAnalytics.LogEvent("Ads", "Full", "Loaded");
        };
        Interstitial.OnAdLeavingApplication += (sender, args) =>
        {
            Debug.Log("Full Clicked");
            FirebaseAnalytics.LogEvent("Ads", "Full", "Clicked");
            API.IsAdsClick = true;
        };
        Interstitial.OnAdFailedToLoad += (sender, args) =>
        {
            IsFullRequesting = false;
            Debug.Log("Full Failed " + args.Message);
            FirebaseAnalytics.LogEvent("Ads", "Full", "Failed");
        };
        Interstitial.OnAdClosed += (sender, args) =>
        {
            DOVirtual.DelayedCall(.01f, () =>
            {
                if (_onFullClosed != null)
                    _onFullClosed();

                Debug.Log("Full Closed");
                FirebaseAnalytics.LogEvent("Ads", "Full", "Closed");

                RequestFull();
            });
        };
        Interstitial.LoadAd(GetAdRequest());
    }

    public void RequestVideo()
    {
        if (IsVideoRequesting)
            return;

        FirebaseAnalytics.LogEvent("Ads", "Banner", "Video");
        RewardBasedVideo.LoadAd(GetAdRequest(), _infor.AdmobVideoId);
    }

    public void ShowBanner()
    {
        if (BannerView != null)
            BannerView.Show();
        else
            RequestBanner();
    }

    public void HideBanner()
    {
        if (BannerView != null)
            BannerView.Hide();
    }

    public void ShowFull(Action closedCallback)
    {
        if (Interstitial.IsLoaded())
        {
            _onFullClosed = closedCallback;
            Interstitial.Show();
        }
        else
        {
            closedCallback();
            RequestFull();
        }
    }

    public void ShowVideo(Action rewardedCallback)
    {
        if (RewardBasedVideo.IsLoaded())
        {
            _onVideoRewarded = rewardedCallback;
            RewardBasedVideo.Show();
        }
        else
            RequestVideo();
    }

    private AdRequest GetAdRequest()
    {
        return new AdRequest.Builder()
            //.AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
            .AddKeyword("game")
            //.SetGender(Gender.Male)
            .SetBirthday(new DateTime(1985, 1, 1))
            .TagForChildDirectedTreatment(false)
            .AddExtra("color_bg", "9B30FF")
            .Build();
    }
}