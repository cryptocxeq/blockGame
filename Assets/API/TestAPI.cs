using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAPI : MonoBehaviour
{
    public Text TxtStatus;
    public void ShowBanner()
    {
        API.Instance.ShowBanner();
    }

    public void HideBanner()
    {
        API.Instance.HideBanner();
    }

    public void ShowFull()
    {
        API.Instance.ShowFull(() => { TxtStatus.text = "Full ads closed"; });
    }

    public void ShowVideo()
    {
        API.Instance.ShowVideo(() => { TxtStatus.text = "Rewarded"; });
    }
}
