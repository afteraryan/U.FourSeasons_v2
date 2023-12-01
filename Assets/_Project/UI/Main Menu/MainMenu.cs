using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnSeasons;
    [SerializeField] private Button btnExit;
    
    private void Awake()
    {
        btnPlay.onClick.AddListener(OnPlayClicked);
        btnSeasons.onClick.AddListener(OnSeasonsClicked);
        btnExit.onClick.AddListener(OnExitClicked);
    }
    
    private void OnPlayClicked()
    {
        SceneManager.LoadScene("Level_01");
    }
    
    private void OnSeasonsClicked()
    {
        Debug.Log("Seasons clicked");
    }
    
    private void OnExitClicked()
    {
        Debug.Log("Exit clicked");
        Application.Quit();
    }
}
