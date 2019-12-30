using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;
    public static bool IsSoundOn = true;
    public static bool IsMusicOn = true;

    public SoundInfor MenuBackgroundSound;
    public SoundInfor InGameBackgroundSound;
    public SoundInfor PickItem;
    public SoundInfor PutItem;
    public SoundInfor Bonus;
    public SoundInfor Firework;
    public SoundInfor NewHighScore;
    public SoundInfor StartLevel;
    public SoundInfor Gameover;

    public List<SoundInfor> Eats;

    private AudioSource _backgroundSource;
    private List<AudioSource> _audioSources = new List<AudioSource>();
    private SoundInfor _curBackgroundMusic = new SoundInfor();

    private void Awake()
    {
        Instance = this;
        _backgroundSource = gameObject.AddComponent<AudioSource>();
        _backgroundSource.loop = true;

        IsSoundOn = true;
        IsMusicOn = true;
    }

    private void Start()
    {
        if (IsMusicOn)
        {
            PlayBackgroundMusic(MenuBackgroundSound);
        }
    }

    public void PlayBackgroundMusic(SoundInfor sound = null)
    {
        if (sound != null)
            _curBackgroundMusic = sound;

        if (IsMusicOn)
        {
            if (sound == null)
            {
                _backgroundSource.clip = _curBackgroundMusic.Clip;
                _backgroundSource.volume = _curBackgroundMusic.Volume;
                _backgroundSource.Play();
            }
            else
            {
                _backgroundSource.clip = sound.Clip;
                _backgroundSource.volume = sound.Volume;
                _backgroundSource.Play();
            }
        }
    }

    public void StopBackgroundMusic()
    {
        _backgroundSource.Stop();
    }

    public void PlayEatSound(int numberline)
    {
        if (!IsSoundOn)
            return;

        AudioSource audioSource = GetAudioSource();
        audioSource.clip = Eats[0].Clip;
        audioSource.volume = Eats[0].Volume;
        audioSource.pitch = 1 + numberline / 10f;
        audioSource.Play();
    }

    public void PlaySoundEffect(SoundInfor sound)
    {
        if (!IsSoundOn)
            return;
        
        AudioSource audioSource = GetAudioSource();
        audioSource.clip = sound.Clip;
        audioSource.volume = sound.Volume;
        audioSource.Play();
    }

    public void StopAllSoundEffect()
    {
    }

    private AudioSource GetAudioSource()
    {
        if (_audioSources.Count > 0)
        {
            for (int i = 0; i < _audioSources.Count; i++)
            {
                if (!_audioSources[i].isPlaying)
                {
                    return _audioSources[i];
                }
            }

            return GetNewAudioSource();
        }

        return GetNewAudioSource();
    }

    private AudioSource GetNewAudioSource()
    {
        return gameObject.AddComponent<AudioSource>();
    }
}

[Serializable]
public class SoundInfor
{
    public AudioClip Clip;
    public float Volume = 1;
}