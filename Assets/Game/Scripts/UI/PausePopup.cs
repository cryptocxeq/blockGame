using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePopup : PageBase {

	public void ButtonMenuClick()
	{
        SaveGame.instance.deleteAll();
        UiManager.Instance.Popup.ButtonMenuClick();
		MyFirebase.LogButtonEvent(MyFirebase.EventButtonName.click_menu_pause);
	}

	public void ButtonContinueClick()
	{
		UiManager.Instance.Popup.ButtonContinueClick();
		MyFirebase.LogButtonEvent(MyFirebase.EventButtonName.click_continue_pause);
	}

	public void ButtonReplayClick()
    {
        SaveGame.instance.deleteAll();
        SaveGame.instance.deleteList();
        UiManager.Instance.Popup.ButtonReplayClick();
		MyFirebase.LogButtonEvent(MyFirebase.EventButtonName.click_replay_pause);
	}
}
