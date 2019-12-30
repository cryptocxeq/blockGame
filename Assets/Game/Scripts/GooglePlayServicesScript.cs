using System;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
#endif
#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GooglePlayServicesScript : MonoBehaviour {
    void Awake()
    {
#if UNITY_ANDROID
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            // enables saving game progress.
            //.EnableSavedGames()
            // registers a callback to handle game invitations received while the game is not running.
            //.WithInvitationDelegate(OnWithInvitation)
            // registers a callback for turn based match notifications received while the
            // game is not running.
            //.WithMatchDelegate(OnWithMatch)
            // require access to a player's Google+ social graph (usually not needed)
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
#endif

#if UNITY_IOS
		IosAuthenticate();
		#endif
    }

    void Start()
    {
#if UNITY_ANDROID
        Signin();
#endif
    }
#if UNITY_ANDROID
    public static void Signin(Action callBack = null)
    {
        // authenticate user:
        Social.localUser.Authenticate((bool success) => {
            // handle success or failure
            Debug.Log("Signin " + success);
            if (callBack != null)
                if (success)
                    callBack.Invoke();
        });
    }

    private void OnWithMatch(TurnBasedMatch match, bool shouldautolaunch)
    {
        Debug.Log("OnWithMatch");
    }

    private void OnWithInvitation(Invitation invitation, bool shouldautoaccept)
    {
        Debug.Log("OnWithInvitation");
    }


#endif

    // IOS----------------------------
    public const string TopHighScore = "BlockMarble_TopHighScore";

#if UNITY_IOS
	private void IosAuthenticate()
	{
		// Authenticate and register a IosProcessAuthentication callback
		// This call needs to be made before we can proceed to other calls in the Social API
		Social.localUser.Authenticate(IosProcessAuthentication);
	}

	private void IosProcessAuthentication(bool success)
	{
		if (success)
		{
			Debug.Log("Authenticated, checking achievements");

			// Request loaded achievements, and register a callback for processing them
			// Social.LoadAchievements(ProcessLoadedAchievements);
		}
		else
			Debug.Log("Failed to authenticate");
	}

	public static void IosReportScore(string leaderBoardId, long score)
	{
		// You can also call into the functions like this
		Social.ReportScore(score, leaderBoardId, b =>
			{
				if (b)
				{
					Debug.Log("Post score success: " + score);
				}
			});
	}

	public static void IosShowLeaderBoard()
	{
		Social.ShowLeaderboardUI();
	}

	#endif


    public static void ReportProgress(string achievementId)
    {
#if UNITY_ANDROID
        if (PlayGamesPlatform.Instance.IsAuthenticated()) {
            Social.ReportProgress(achievementId, 100.0f, (bool success) => { });
        }
#endif
    }


    public static void IncrementingAnAchievement(string achievementId, int value, Action successCalback)
    {
#if UNITY_ANDROID
        if (PlayGamesPlatform.Instance.IsAuthenticated()) {
            PlayGamesPlatform.Instance.IncrementAchievement(
                achievementId, value, (bool success) => {
                    if (success) {
                        if (successCalback != null) {
                            successCalback();
                        }
                    }

                    Debug.Log("IncrementingAnAchievement " + success);
                });
        }
#endif
    }

    public static void UnlockAchievement(string achievementId, Action successCalback)
    {
#if UNITY_ANDROID
        if (PlayGamesPlatform.Instance.IsAuthenticated()) {
            PlayGamesPlatform.Instance.UnlockAchievement(achievementId, b => {
                if (b) {
                    if (successCalback != null) {
                        successCalback();
                    }
                }
            });
        }
#endif
#if UNITY_IOS
        Social.ReportProgress(achievementId, 100, b =>
        {
            if (b)
            {
                if (successCalback!=null)
                {
                    successCalback();
                }
            }
        });
#endif
    }

    public static void ReportScore(string leaderboardId, int score)
    {
#if UNITY_ANDROID
        if (PlayGamesPlatform.Instance.IsAuthenticated()) {
            Social.ReportScore(score, leaderboardId, (bool success) => { Debug.Log("ReportScore " + success); });
        }
#endif

#if UNITY_IOS
		Social.ReportScore(score, leaderboardId, b =>
			{
				if (b)
				{
					Debug.Log("Post score success: " + score);
				}
			});
		#endif
    }

    public static void ShowLeaderboardUI(string leaderboardId = "")
    {
#if UNITY_ANDROID
        if (PlayGamesPlatform.Instance.IsAuthenticated()) {
            if (leaderboardId == string.Empty)
                Social.ShowLeaderboardUI();
            else
                PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardId);
        }
        else {
            Signin();
        }
#endif

#if UNITY_IOS
		Social.ShowLeaderboardUI();
		#endif
    }

    public static void ShowAchievementsUI()
    {
#if UNITY_ANDROID
        if (PlayGamesPlatform.Instance.IsAuthenticated()) {
            Social.ShowAchievementsUI();
        }
        else {
            Signin();
        }
#endif
#if UNITY_IOS
		Social.ShowAchievementsUI();
		#endif
    }
}