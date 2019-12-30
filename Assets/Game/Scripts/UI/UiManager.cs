using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [Header("Pages")] public HomePage HomePage;
    public GamePage GamePage;
    public PopupController Popup;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HomePage.gameObject.SetActive(true);
        GamePage.gameObject.SetActive(false);
        Popup.gameObject.SetActive(false);

        StartCoroutine(CheckDailyBonus());
    }

    [ContextMenu("RemoveAllPlayerPref")]
    public void RemoveAllPlayerPref()
    {
        PlayerPrefs.DeleteAll();
    }

    public IEnumerator CheckDailyBonus()
    {
        yield return new WaitForSeconds(3);

        string data = PlayerPrefs.GetString("CurDay", string.Empty);
        if (data != String.Empty)
        {
            Debug.Log("DataTimeNow: " + DateTime.Now.ToShortDateString());
            if (data != DateTime.Now.ToShortDateString())
            {
                PlayerPrefs.SetString("CurDay", DateTime.Now.ToShortDateString());

                GameController.Instance.AddMoreRotate(3);
                NotifyController.Instance.ShowNotify(string.Format("A new day bonus {0} rotate!", 3));
            }
        }
        else
        {
            PlayerPrefs.SetString("CurDay", DateTime.Now.ToShortDateString());
        }
    }
}