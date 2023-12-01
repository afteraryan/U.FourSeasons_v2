using UnityEngine;

public class CameraOffsetVolume : MonoBehaviour
{
    [SerializeField] private float targetOrthographicSize = 6f;
    [SerializeField] private Vector2 targetOffset;
    [SerializeField] private float transitionDuration = 0.75f;
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Header("CONTROLS")]
    [SerializeField] private bool disableControls = false;

    private CameraMovement _cameraMovement;
    private Transform _playerTransform;

    private void Awake()
    {
        spriteRenderer.enabled = false;
    }

    private void Start()
    {
        _cameraMovement = FindObjectOfType<CameraMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _cameraMovement.OnEnterCameraOffsetVolume(targetOrthographicSize, targetOffset, transitionDuration, disableControls);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _cameraMovement.OnExitCameraOffsetVolume(transitionDuration/3);
    }
}