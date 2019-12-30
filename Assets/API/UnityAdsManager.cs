using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsManager
{
    private Action _onFinish;
    private Action _onSkipped;
    private Action _onFailed;

    public void Init(string id)
    {
        Advertisement.Initialize(id, false);
    }

    public static bool IsRewardedAdReady()
    {
        return Advertisement.IsReady("rewardedVideo");
    }

    public void ShowRewardedAd(Action onFinish = null, Action onSkipped = null, Action onFailed = null)
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            _onFailed = null;
            _onFinish = null;
            _onSkipped = null;

            _onFailed = onFailed;
            _onFinish = onFinish;
            _onSkipped = onSkipped;
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
        else
            Debug.Log("Video is unavaiable!");
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                FirebaseAnalytics.LogEvent("Ads", "UnityAds", "Finish");
                if (_onFinish != null)
                    _onFinish.Invoke();
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                FirebaseAnalytics.LogEvent("Ads", "UnityAds", "Skip");
                if (_onSkipped != null)
                    _onSkipped.Invoke();
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                FirebaseAnalytics.LogEvent("Ads", "UnityAds", "Failed");
                if (_onFailed != null)
                    _onFailed.Invoke();
                break;
        }
    }

    public bool IsVideoAvailable()
    {
        return Advertisement.IsReady();
    }
}
