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
    private Animator _cameraAnimator;
    private static readonly int TriggerCameraShake = Animator.StringToHash("trigger_CameraShake");

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
        _cameraAnimator = GetComponentInChildren<Animator>();
        _initialPosition = transform.position;
        _camera.orthographicSize = defaultSize;
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
    }

    private void ShakeCamera()
    {
        _cameraAnimator.SetTrigger("trigger_CameraShake");
    }

    #region ZOOM
    public void ZoomIn()
    {
        StopAllCoroutines(); // Ensure no other zoom coroutines are running
        StartCoroutine(ZoomRoutine(fallSize));
    }
    public void ZoomOut()
    {
        StopAllCoroutines(); // Ensure no other zoom coroutines are running
        StartCoroutine(ZoomRoutine(jumpSize));
    }
    public void ResetZoom()
    {
        ShakeCamera();
        StopAllCoroutines(); // Ensure no other zoom coroutines are running
        StartCoroutine(ZoomRoutine(defaultSize));
    }
    private IEnumerator ZoomRoutine(float targetSize)
    {
        float currentSize = _camera.orthographicSize;
        float time = 0f;

        while (time < zoomDuration)
        {
            _camera.orthographicSize = Mathf.Lerp(currentSize, targetSize, time / zoomDuration);
            time += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        _camera.orthographicSize = targetSize; // Ensure the camera size is set to the target size
    }
    #endregion
}