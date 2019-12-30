using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PopupController : PageBase
{
    [SerializeField] private PausePopup _pausePopup;
    [SerializeField] private GameoverPopup _gameoverPopup;
    [SerializeField] private GameObject _bestScoreEffect;
    [SerializeField] private GameObject _newRewordEffect;

    private int _countGameover
    {
        get { return PlayerPrefs.GetInt("count_gameover"); }
        set { PlayerPrefs.SetInt("count_gameover", value); }
    }

    public void ShowPausePopup()
    {
        Time.timeScale = 0;
        HideAll();
        _pausePopup.Show();
    }

    public void ShowGameoverPopup()
    {
        // Check score to bonus
        if (GameController.Score >= 1000)
        {
            _countGameover++;
        }

        bool isBonusHighScore = true;
        if (_countGameover >= 10)
        {
            int bonus = Random.Range(3, 6);
            GameController.Instance.AddMoreRotate(bonus);
            NotifyController.Instance.ShowNotify(string.Format("You have bonus {0} rotate!", bonus));
            MyFirebase.LogUserProfile(MyFirebase.UserProfile.get_level_bonus);
            isBonusHighScore = false;
        }

        HideAll();
        // Check best score
        if (GameController.Score > GameController.BestScore)
        {
            // store best score
            GameController.BestScore = GameController.Score;

            SaveGame.instance.saveBestScore(GameController.Score);

            // Show effect best score
            _bestScoreEffect.gameObject.SetActive(true);
            _newRewordEffect.SetActive(true);

            SoundController.Instance.PlaySoundEffect(SoundController.Instance.Firework);
            SoundController.Instance.PlaySoundEffect(SoundController.Instance.NewHighScore);

            MyFirebase.LogUserProfile(MyFirebase.UserProfile.get_new_highscore);

            if (isBonusHighScore)
            {
                GameController.Instance.AddMoreRotate(3);
                NotifyController.Instance.ShowNotify(string.Format("You have bonus {0} rotate!", 3));
            }


            if (Application.platform == RuntimePlatform.Android)
            {
                GooglePlayServicesScript.ReportScore(GPGSIds.leaderboard_top_best_score, GameController.BestScore);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                GooglePlayServicesScript.ReportScore(GooglePlayServicesScript.TopHighScore, GameController.BestScore);
            }
        }
        else
        {
            _bestScoreEffect.SetActive(false);
            _newRewordEffect.SetActive(false);
            //SoundController.Instance.PlaySoundEffect(SoundController.Instance.Gameover);
        }

        Show();
        _gameoverPopup.Show();
        SaveGame.instance.saveScore(0);
    }

    private void HideAll()
    {
        Show();
        _pausePopup.gameObject.SetActive(false);
        _gameoverPopup.gameObject.SetActive(false);
    }

    // Button events
    public void ButtonMenuClick()
    {
        API.Instance.ShowFull(() =>
        {
            Time.timeScale = 1;

            if (_pausePopup.gameObject.activeInHierarchy)
            {
                CheckHighScoreWhenReplay();
            }

            GameController.Instance.StopGame();
            for (int i = 0; i < GameController.Instance.BoardItems.Count; i++)
            {
                GameController.Instance.BoardItems[i].restart();
            }
            Hide();

            UiManager.Instance.GamePage.Hide();
            UiManager.Instance.HomePage.Show();

            SoundController.Instance.PlayBackgroundMusic(SoundController.Instance.MenuBackgroundSound);
        });
    }

    public void ButtonContinueClick()
    {
        API.Instance.ShowFull(ContinueGame);
    }

    private void ContinueGame()
    {
        Time.timeScale = 1;
        Hide();
    }

    public void ButtonReplayClick()
    {
        API.Instance.ShowFull(ReplayGame);
    }

    private void ReplayGame()
    {
        Time.timeScale = 1;

        if (_pausePopup.gameObject.activeInHierarchy)
        {
            CheckHighScoreWhenReplay();
        }

        GameController.Instance.StopGame();
        for (int i = 0; i < GameController.Instance.BoardItems.Count; i++)
        {
            GameController.Instance.BoardItems[i].restart();
        }
        Hide();
        GameController.Instance.StartGame();
    }

    public void CheckHighScoreWhenReplay()
    {
        if (GameController.Score > GameController.BestScore)
        {
            // store best score
            GameController.BestScore = GameController.Score;
            SaveGame.instance.saveBestScore(GameController.Score);
            MyFirebase.LogUserProfile(MyFirebase.UserProfile.get_new_highscore);

            if (Application.platform == RuntimePlatform.Android)
            {
                GooglePlayServicesScript.ReportScore(GPGSIds.leaderboard_top_best_score, GameController.BestScore);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                GooglePlayServicesScript.ReportScore(GooglePlayServicesScript.TopHighScore, GameController.BestScore);
            }

            GameController.Instance.AddMoreRotate(2);
            NotifyController.Instance.ShowNotify(string.Format("You have bonus {0} rotate from high score!", 2));
        }

        SaveGame.instance.saveScore(0);
    }
}