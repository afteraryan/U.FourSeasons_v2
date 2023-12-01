using System.Collections;
using UnityEngine;
using CharacterController = _Project.Player_Character.Script.CharacterController;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    [Header("ZOOM PARAMETERS")]
    [SerializeField] private float zoomDuration = 3f;
    [SerializeField] private float defaultSize = 5f;
    [SerializeField] private float jumpSize = 6f;
    [SerializeField] private float fallSize = 4f;

    private Vector3 _initialPosition;
    private Camera _camera;
    private Vector3 _cameraOriginalLocalPosition;
    private Animator _cameraAnimator;
    private static readonly int TriggerCameraShake = Animator.StringToHash("trigger_CameraShake");
    private Coroutine _currentRoutine;
    private CharacterController _characterController;

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
        _cameraAnimator = GetComponentInChildren<Animator>();
        _initialPosition = transform.position;
        _camera.orthographicSize = defaultSize;
        _cameraOriginalLocalPosition = _camera.transform.localPosition;
        _characterController = FindObjectOfType<CharacterController>();
    }

    private void Start()
    {
        DisableCameraAnimator();
    }

    private void OnEnable()
    {
        CharacterController.OnJump += ZoomOut;
        CharacterController.OnFall += ZoomIn;
        CharacterController.OnLand += ResetZoom;
    }
    
    private void OnDisable()
    {
        CharacterController.OnJump -= ZoomOut;
        CharacterController.OnFall -= ZoomIn;
        CharacterController.OnLand -= ResetZoom;
    }

    private void FixedUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        transform.position = new Vector3(_initialPosition.x, target.position.y, _initialPosition.z);
        //transform.position = new Vector3(target.position.x, target.position.y, _initialPosition.z);
    }

    private void ShakeCamera()
    {
        _cameraAnimator.enabled = true;
        _cameraAnimator.SetTrigger(TriggerCameraShake);
        Invoke(nameof(DisableCameraAnimator), 0.5f);
    }
    
    private void DisableCameraAnimator()
    {
        _cameraAnimator.enabled = false;
    }

    #region ZOOM
    private void ZoomIn()
    {
        if(_isInCameraOffsetVolume)
            return;
        
        if (_currentRoutine != null)
            StopCoroutine(_currentRoutine);
        
        _currentRoutine = StartCoroutine(ZoomRoutine(fallSize));
    }
    private void ZoomOut()
    {
        if(_isInCameraOffsetVolume)
            return;
        if (_currentRoutine != null)
            StopCoroutine(_currentRoutine);
        
        _currentRoutine = StartCoroutine(ZoomRoutine(jumpSize));
    }
    private void ResetZoom()
    {
        if(_isInCameraOffsetVolume)
            return;
        ShakeCamera();
        
        if (_currentRoutine != null)
            StopCoroutine(_currentRoutine);
        
        _currentRoutine = StartCoroutine(ZoomRoutine(defaultSize));
    }
    private IEnumerator ZoomRoutine(float targetSize)
    {
        Debug.Log("Zoom");
        float currentSize = _camera.orthographicSize;
        float time = 0f;

        while (time < zoomDuration)
        {
            _camera.orthographicSize = Mathf.Lerp(currentSize, targetSize, time / zoomDuration);
            time += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        _camera.orthographicSize = targetSize; // Ensure the camera size is set to the target size
        _currentRoutine = null;
    }
    #endregion
    
    #region CAMERA OFFSET VOLUME
    
    private bool _isInCameraOffsetVolume = false;
    public void OnEnterCameraOffsetVolume(float newSize, Vector2 offset, float duration, bool controlDisabled)
    {
        _characterController.SetInputEnabled(!controlDisabled);
        _isInCameraOffsetVolume = true;
        if (_currentRoutine != null)
            StopCoroutine(_currentRoutine);
        
        _currentRoutine = StartCoroutine(ChangeCameraProperties(newSize, offset, duration, true));
    }

    public void OnExitCameraOffsetVolume(float duration)
    {
        if (_currentRoutine != null)
            StopCoroutine(_currentRoutine);

        _currentRoutine = StartCoroutine(ChangeCameraProperties(defaultSize, Vector2.zero, duration, false));
    }

    private IEnumerator ChangeCameraProperties(float targetSize, Vector2 targetOffset, float duration, bool isEnteringVolume)
    {
        float time = 0f;
        Vector3 startLocalPosition = _camera.transform.localPosition;
        float startSize = _camera.orthographicSize;
        Vector3 targetLocalPosition = isEnteringVolume ? new Vector3(targetOffset.x, targetOffset.y, startLocalPosition.z) : _cameraOriginalLocalPosition;

        while (time < duration)
        {
            _camera.orthographicSize = Mathf.Lerp(startSize, targetSize, time / duration);
            _camera.transform.localPosition = Vector3.Lerp(startLocalPosition, targetLocalPosition, time / duration);
            time += Time.deltaTime; 
            yield return null;
        }
        _camera.orthographicSize = targetSize;
        _camera.transform.localPosition = targetLocalPosition;
        _isInCameraOffsetVolume = isEnteringVolume;
        _currentRoutine = null;
        _characterController.SetInputEnabled(true);
    }

    
    
    #endregion
}