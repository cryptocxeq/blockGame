using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NotifyController : MonoBehaviour
{
    public static NotifyController Instance;
    public RectTransform NotifyRect;
    public Text TxtContent;

    public string StrTest = "Test notify!!!";
    private void Awake()
    {
        Instance = this;
        NotifyRect.gameObject.SetActive(false);
    }

//    [ContextMenu("ShowNotify")]
//    public void ShowTest()
//    {
//        ShowNotify(StrTest);
//    }
//
//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            ShowTest();
//        }
//    }

    public void ShowNotify(string content)
    {
        NotifyRect.gameObject.SetActive(true);
        NotifyRect.DOKill();
        TxtContent.text = content;
        NotifyRect.anchoredPosition = new Vector3(NotifyRect.rect.width + 40, 0);
        NotifyRect.DOAnchorPosX(0, .3f).OnComplete(() =>
        {
            NotifyRect.DOAnchorPosX(NotifyRect.rect.width + 40, .3f).SetDelay(3)
                .OnComplete(() => { NotifyRect.gameObject.SetActive(false); });   
        });
    }

   
}