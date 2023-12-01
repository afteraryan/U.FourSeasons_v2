using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = _Project.Player_Character.Script.CharacterController;

public class TouchControlManager : MonoBehaviour
{
    
    //PRIMARY TOUCH PROPERTIES
    [Header("PRIMARY TOUCH PROPERTIES")]
    [SerializeField] private float touchDistanceThreshold = 0.1f;
    private float _touchStartTime;
    private Vector2 _touchStartPosition;
    private float _touchEndTime;
    private Vector2 _touchEndPosition;

    private const float TouchSensitivity = 0.6f; // Lower this value to reduce sensitivity
    private const float MovementThreshold = 8f; // Adjust this threshold as needed

    
    private float _previousTouchPosition;
    private float _currentTouchPosition;
    private float _initialTouchPosition;
    private Coroutine _touchMovementCoroutine;
    private float _horizontalInput;
    private bool _isTouching;
    
    private CharacterController _characterController;

    private void OnEnable()
    {
        PlayerInputManager.OnPrimaryTouchStarted += OnPrimaryTouchStarted;
        PlayerInputManager.OnPrimaryTouchCanceled += OnPrimaryTouchCanceled;
        PlayerInputManager.OnTapPerformed += OnTapPerformed;
    }

    private void OnDisable()
    {
        PlayerInputManager.OnPrimaryTouchStarted -= OnPrimaryTouchStarted;
        PlayerInputManager.OnPrimaryTouchCanceled -= OnPrimaryTouchCanceled;
        PlayerInputManager.OnTapPerformed -= OnTapPerformed;
    }

    private void Awake()
    {
        _characterController = FindObjectOfType<CharacterController>();
    }

    
    private void OnPrimaryTouchStarted(Vector2 touchPosition, float time)
    {
        if (!_characterController.GetInputEnabled())
            return;
        _isTouching = true;
        _touchStartTime = time;
        _initialTouchPosition = touchPosition.x;
        _previousTouchPosition = _initialTouchPosition;
        _characterController.Jump();

        // Start the touch movement coroutine
        _touchMovementCoroutine = StartCoroutine(TouchMovementCoroutine());
    }
    
    private void OnPrimaryTouchCanceled(Vector2 touchPosition, float time)
    {
        if (!_characterController.GetInputEnabled())
            return;
        _isTouching = false;
        _touchEndTime = time;
        _characterController.SetHorizontalInput(0);
        
        // Stop the touch movement coroutine when the touch ends
        if (_touchMovementCoroutine != null)
        {
            StopCoroutine(_touchMovementCoroutine);
            _touchMovementCoroutine = null;
        }
    }
    private IEnumerator TouchMovementCoroutine()
    {
        while (_isTouching)
        {
            // Retrieve the current touch position
            float currentTouchPosition = PlayerInputManager.Instance._playerInput.Touch.XPosition.ReadValue<float>();
            
            // Calculate the change in position
            float deltaX = currentTouchPosition - _previousTouchPosition;

            // Apply the sensitivity factor
            deltaX *= TouchSensitivity;

            // Check if the movement is above the threshold to reduce sensitivity
            if (Mathf.Abs(deltaX) > MovementThreshold)
            {
                // Determine the direction of the movement
                float direction = Mathf.Sign(deltaX);

                // Set the horizontal input based on the direction
                _horizontalInput = direction;

                // Call the function that moves the player character
                _characterController.SetHorizontalInput(_horizontalInput);
            }

            // Update the previous touch position
            _previousTouchPosition = currentTouchPosition;

            // Wait until the next frame
            yield return null;
        }

        // When the touch ends or is not active, reset the horizontal input
        _horizontalInput = 0;
        _characterController.SetHorizontalInput(_horizontalInput);
    }

    private void OnTapPerformed()
    {
        if (!_characterController.GetInputEnabled())
            return;
        //_characterController.Jump();
    }
}
