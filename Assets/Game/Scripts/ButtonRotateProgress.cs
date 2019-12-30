using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ButtonRotateProgress : MonoBehaviour
{
    public static ButtonRotateProgress Instance;
    public float CurValue;
    public float MaxValue;
    public Image ImgProgress;
    private Tween _changeTween;

    [SerializeField] private Transform _txtNumberRotate;

    private void Awake()
    {
        Instance = this;
        ImgProgress.fillAmount = 0;
    }

    public void ChangeValue(float add)
    {
        CurValue += add;
        if (_changeTween != null)
            _changeTween.Kill();
        float duration = 4f;
        _changeTween = DOVirtual.Float(ImgProgress.fillAmount * MaxValue, CurValue, duration, value =>
        {
            UpdateValue(value);
            if (value >= MaxValue)
            {
                _changeTween.Kill();
                GameController.Instance.NumRotate++;
                ImgProgress.fillAmount = 0;
                CurValue -= MaxValue;
//                _changeTween = DOVirtual.Float(ImgProgress.fillAmount * MaxValue, CurValue, duration,
//                    value2 => { UpdateValue(value); }).SetSpeedBased(true).SetEase(Ease.Linear);
                ImgProgress.fillAmount = 0;
                _txtNumberRotate.DOScale(1.2f, .3f).SetLoops(2, LoopType.Yoyo);
            }
        }).SetSpeedBased(true).SetEase(Ease.Linear);
    }


    private void UpdateValue(float value)
    {
        ImgProgress.fillAmount = value / MaxValue;
    }
}