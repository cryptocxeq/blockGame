using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomePage : PageBase {

	public void ButtonPlayClick()
	{
		Hide();
		UiManager.Instance.GamePage.Show();
		PlayGame();	
	}

	public void PlayGame()
	{
		GameController.Instance.StartGame();
		SoundController.Instance.PlayBackgroundMusic(SoundController.Instance.InGameBackgroundSound);
		MyFirebase.LogButtonEvent(MyFirebase.EventButtonName.click_play_in_home);
	}

	public void ButtonMoregameClick()
	{
		API.Instance.GotoStore();
		MyFirebase.LogButtonEvent(MyFirebase.EventButtonName.click_moregame);
	}

	public void ButtonRateClick()
	{
		API.Instance.RateApp();
		MyFirebase.LogButtonEvent(MyFirebase.EventButtonName.click_rate_home);
	}
}
