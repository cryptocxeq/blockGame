using DG.Tweening;
using UnityEngine.UI;

public class GameoverPopup : PageBase
{
    public Text TxtBestScore;
    public Text TxtScore;

    private Tween _scoreTween;

    private void OnEnable()
    {
        TxtBestScore.text = GameController.BestScore.ToString();
        SaveGame.instance.saveBestScore(GameController.BestScore);
        int score = GameController.Score;
        _scoreTween = DOVirtual.Float(0, score, .3f, value => { TxtScore.text = ((int) value).ToString(); })
            .SetUpdate(true).SetEase(Ease.Linear);
    }

    private void OnDisable()
    {
        if (_scoreTween != null)
        {
            if (_scoreTween.IsPlaying())
            {
                _scoreTween.Kill();
            }
        }
    }

    public void ButtonMenuClick()
    {
        UiManager.Instance.Popup.ButtonMenuClick();
        MyFirebase.LogButtonEvent(MyFirebase.EventButtonName.click_menu_gameover);
    }

    public void ButtonReplayClick()
    {
        UiManager.Instance.Popup.ButtonReplayClick();
        MyFirebase.LogButtonEvent(MyFirebase.EventButtonName.click_replay_gameover);
    }
}