using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : GenericMonoSingleton<PlayerInputManager>
{
    //private InputSystem _playerInput;
    [HideInInspector]public PlayerControls _playerInput;

    #region Events

    public delegate void PrimaryTouchStartedDelegate(Vector2 touchPosition, float time);
    public static event PrimaryTouchStartedDelegate OnPrimaryTouchStarted;
    public delegate void PrimaryTouchCanceledDelegate(Vector2 touchPosition, float time);
    public static event PrimaryTouchCanceledDelegate OnPrimaryTouchCanceled;
    public delegate void TapPerformedDelegate();
    public static event TapPerformedDelegate OnTapPerformed;

    #endregion

    private void Awake()
    {
        //_playerInput = new InputSystem();
        _playerInput = new PlayerControls();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
        _playerInput.Touch.PrimaryTouch.started += PrimaryTouchStarted;
        _playerInput.Touch.PrimaryTouch.canceled += PrimaryTouchCanceled;
        _playerInput.Touch.Tap.performed += TapPerformed;
    }

    private void OnDisable()
    {
        _playerInput.Disable();
        _playerInput.Touch.PrimaryTouch.started -= PrimaryTouchStarted;
        _playerInput.Touch.PrimaryTouch.canceled -= PrimaryTouchCanceled;
        _playerInput.Touch.Tap.performed -= TapPerformed;
    }
    
    private void PrimaryTouchStarted(InputAction.CallbackContext ctx)
    {
        if(OnPrimaryTouchStarted != null)
            OnPrimaryTouchStarted?.Invoke(_playerInput.Touch.PrimaryPosition.ReadValue<Vector2>() , (float)ctx.startTime);
    }
    private void PrimaryTouchCanceled(InputAction.CallbackContext ctx)
    {
        if(OnPrimaryTouchCanceled != null)
            OnPrimaryTouchCanceled?.Invoke(_playerInput.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)ctx.time);
    }
    
    private void TapPerformed(InputAction.CallbackContext ctx)
    {
        OnTapPerformed?.Invoke();   
    }
}
