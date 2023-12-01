using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CharacterController = _Project.Player_Character.Script.CharacterController;

public class GameSessionManager : MonoBehaviour
{
    [Header("GAME OVER SCREEN")]
    [SerializeField] private Canvas canvasGameOver;
    [SerializeField] private Button buttonRestart;
    
    #region EVENT FUNCTIONS
    private void OnEnable()
    {
        CharacterController.PlayerDied += OnPlayerDied;
    }
    private void OnDisable()
    {
        CharacterController.PlayerDied -= OnPlayerDied;
    }

    private void Awake()
    {
        SetButtonListeners();
    }

    #endregion
    
    private void OnPlayerDied()
    {
        canvasGameOver.gameObject.SetActive(true);
    }

    #region GAME OVER SCREEN
    private void SetButtonListeners()
    {
        buttonRestart.onClick.AddListener(RestartGameSession);
    }
    #endregion
    
    #region GAME FUNCTIONS
    private void RestartGameSession()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
}
