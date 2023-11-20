using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAudioManager : GenericMonoSingleton<LevelAudioManager>
{
    [SerializeField] private float fadeDuration = 2.0f; // Duration of the fade
    [SerializeField] private AudioSource backgroundMusicSource;
    private float _defaultVolume;

    private void Awake()
    {
        _defaultVolume = backgroundMusicSource.volume;
    }

    public void PlayBackgroundMusic(AudioClip audioClip)
    {
        backgroundMusicSource.clip = audioClip;
        backgroundMusicSource.Play();
    }
    
    public void StopBackgroundMusic()
    {
        backgroundMusicSource.Stop();
    }
    
    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }
    
    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    

    private IEnumerator FadeInCoroutine()
    {
        float currentTime = 0;
        float startVolume = 0;

        backgroundMusicSource.volume = startVolume;
        backgroundMusicSource.Play();

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            backgroundMusicSource.volume = Mathf.Lerp(startVolume, _defaultVolume, currentTime / fadeDuration);
            yield return null;
        }

        backgroundMusicSource.volume = _defaultVolume;
    }

    private IEnumerator FadeOutCoroutine()
    {
        float currentTime = 0;
        float startVolume = backgroundMusicSource.volume;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            backgroundMusicSource.volume = Mathf.Lerp(startVolume, 0, currentTime / fadeDuration);
            yield return null;
        }

        backgroundMusicSource.Stop();
        backgroundMusicSource.volume = startVolume;
    }
}
