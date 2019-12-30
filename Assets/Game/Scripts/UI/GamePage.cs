using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GamePage : PageBase
{
    public Text TxtScore;
    public Text TxtBestScore;
    private Tween _scoreTween;


    private void Start()
    {
        TxtScore.text = GameController.Score.ToString();

        TxtBestScore.text = PlayerPrefs.GetInt("bestScore").ToString();
        //TxtBestScore.text = GameController.BestScore.ToString();
        GameController.OnBestScoreChanged += () => { TxtBestScore.text = GameController.BestScore.ToString(); };

        GameController.OnScoreChanged += () =>
        {
            TxtScore.DOKill();
            _scoreTween = DOVirtual.Float(int.Parse(TxtScore.text), GameController.Score, .3f,
                f => { TxtScore.text = ((int) f).ToString(); }).SetEase(Ease.Linear);
        };
    }

    public void ButtonPauseClick()
    {
        UiManager.Instance.Popup.ShowPausePopup();
        MyFirebase.LogButtonEvent(MyFirebase.EventButtonName.click_pause_button);
    }
}