using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGetMoreRotate : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (API.Instance.IsHasVideo())
            API.Instance.ShowVideo(() =>
            {
                GameController.Instance.AddMoreRotate(3);
                NotifyController.Instance.ShowNotify("You got 3 rotate!");
                MyFirebase.LogButtonEvent(MyFirebase.EventButtonName.click_reward__get_rotate);
            });
        else
        {
            // Notify
        }
    }
}