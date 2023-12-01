using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicVolume : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (LevelAudioManager.Instance == null)
            return;
        LevelAudioManager.Instance.PlayBackgroundMusic(audioClip);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (LevelAudioManager.Instance == null)
            return;
        LevelAudioManager.Instance.StopBackgroundMusic();
    }
}
