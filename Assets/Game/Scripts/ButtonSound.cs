using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
	[SerializeField]
	private Image _img;
	
	[SerializeField]
	private Sprite OnSprite, OffSprite;

	[SerializeField]
	private bool _isButtonSound;

	private void OnEnable()
	{
		if (GameController.Instance!=null)
		{
			UpdateState();
		}
	}

	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(OnClick);
		UpdateState();
	}
	
	private void OnClick()
	{
		if (_isButtonSound)
		{
			SoundController.IsSoundOn = !SoundController.IsSoundOn;
			if (SoundController.IsSoundOn == false)
			{
				SoundController.Instance.StopAllSoundEffect();
			}

			if (SoundController.IsSoundOn)
			{
				MyFirebase.LogButtonEvent(MyFirebase.EventButtonName.click_on_sound);
			}
			else
			{
				MyFirebase.LogButtonEvent(MyFirebase.EventButtonName.click_off_sound);				
			}
		}
		else
		{
			SoundController.IsMusicOn = !SoundController.IsMusicOn;
			if (SoundController.IsMusicOn == false)
			{
				SoundController.Instance.StopBackgroundMusic();
			}
			else
			{
				SoundController.Instance.PlayBackgroundMusic();
			}
			
			if (SoundController.IsMusicOn)
			{
				MyFirebase.LogButtonEvent(MyFirebase.EventButtonName.click_on_music);
			}
			else
			{
				MyFirebase.LogButtonEvent(MyFirebase.EventButtonName.click_off_music);				
			}
		}
		UpdateState();
	}

	private void UpdateState()
	{
		if (_isButtonSound)
		{
			SetState(SoundController.IsSoundOn);
		}
		else
		{
			SetState(SoundController.IsMusicOn);
		}
	}

	private void SetState(bool isOn)
	{
		_img.sprite = isOn ? OnSprite : OffSprite;
		_img.SetNativeSize();
	}
}
