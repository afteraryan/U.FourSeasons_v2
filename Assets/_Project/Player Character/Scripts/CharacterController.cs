using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Player_Character.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Player_Character.Script
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private float jumpVelocity;
        [SerializeField] private float horizontalSpeed;
        [SerializeField] private float gravity;
        private float _currentGravity;
        [SerializeField] private float maxJumpHeight;
        private float _verticalSpeed;
        
        [Header("PHYSICS PARAMETERS")]
        [SerializeField] private float jumpImpulseMultiplier = 1f;
        [SerializeField] private float jumpImpulseDuration = 0f;
        [SerializeField] private float fallImpulseMultiplier = 1f;
        [SerializeField] private float fallImpulseDuration = 0f;
        private float _verticalImpulseMultiplier = 1f;
        private float _horizontalImpulseMultiplier = 1f;
        private float _boostExtraVelocity;
        private float _flowerBoostImpulse = 1f;
        private float _flowerBoostHeightMultiplier = 1f;
        
        [Header("Ground Check Parameters")]
        [SerializeField] private Transform groundCheckPointCenter;
        [SerializeField] private Transform groundCheckPointLeft;
        [SerializeField] private Transform groundCheckPointRight;
        [SerializeField] private LayerMask whatIsGround;
        [Header("Raycast Ground Check Parameters")]
        [SerializeField] private float raycastLength = 0.3f;
        [SerializeField] private float raycastSideOffset = 0.5f;
        
        //JUMP and FALL
        private bool _isGrounded;
        private bool _isJumping;
        private float _startJumpHeight;
        private bool _isFalling;
        private bool _fallStart;
        private float _extraHeight;
        
        //INPUT
        private InputSystem _playerInput;
        private Vector2 _moveInput;
        private float _horizontalInput;
        
        //ACTIONS
        public static Action OnJump;
        public static Action OnLand;
        public static Action OnFall;
        
        //ANIMATIONS
        private AnimationController _animationController;
        private Vector3 _originalScale;
        
        private void Awake()
        {
            _playerInput = new InputSystem();
            _currentGravity = gravity;
            _originalScale = transform.localScale;
            
            _animationController = new AnimationController();
            _animationController.Init(GetComponentInChildren<Animator>());
        }

        private void Start()
        {
            _animationController.UpdateMovementState(CharacterMovementState.IDLE);
        }

        private void OnEnable()
        {
            _playerInput.Enable();
            _playerInput.PlayerCharacter2D.Movement.performed += MovementPerformed;
            _playerInput.PlayerCharacter2D.Movement.canceled += MovementCanceled;
            _playerInput.PlayerCharacter2D.Jump.performed += ctx => Jump();
        }

        private void OnDisable()
        {
            _playerInput.Disable();
            _playerInput.PlayerCharacter2D.Movement.performed -= MovementPerformed;
            _playerInput.PlayerCharacter2D.Movement.canceled -= MovementCanceled;
            _playerInput.PlayerCharacter2D.Jump.performed -= ctx => Jump();
        }

        private void Update()
        {
            GroundCheck();
            if (_isGrounded)
            {
                if (_isJumping)
                {
                    // If grounded and was jumping, reset jump status
                    //_isJumping = false;
                }
                else
                {
                    _verticalSpeed = 0f; // Reset vertical speed if grounded
                }

                if (_isFalling)
                {
                    _isFalling = false;
                    _fallStart = false;
                    OnLand?.Invoke();
                    _animationController.UpdateMovementState(CharacterMovementState.JUMP_LAND);
                }
            }
            else
            {
                // Apply gravity
                _verticalSpeed -= _currentGravity * Time.deltaTime;
                if (_verticalSpeed < 0 && !_fallStart)
                {
                    // If we are moving downwards, consider it as falling
                    _isJumping = false;
                    _isFalling = true;
                    _fallStart = true;
                    _extraHeight = 0;
                    _currentGravity = 3 * gravity;
                    OnFall?.Invoke();
                    _animationController.UpdateMovementState(CharacterMovementState.JUMP_FALL);
                }
            }
        }


        private void FixedUpdate()
        {
            Move();
            HandleFall();
        }

        private void GroundCheck()
        {
            Vector2 centerPoint = groundCheckPointCenter.position;
            Vector2 leftPoint = groundCheckPointLeft.position;
            Vector2 rightPoint = groundCheckPointRight.position;
            
            _isGrounded = Physics2D.Raycast(centerPoint, Vector2.down, raycastLength, whatIsGround) ||
                          Physics2D.Raycast(leftPoint, Vector2.down, raycastLength, whatIsGround) ||
                          Physics2D.Raycast(rightPoint, Vector2.down, raycastLength, whatIsGround);
            
            // Debug rays
            Debug.DrawRay(centerPoint, Vector2.down * raycastLength, _isGrounded ? Color.green : Color.red);
            Debug.DrawRay(leftPoint, Vector2.down * raycastLength, _isGrounded ? Color.green : Color.red);
            Debug.DrawRay(rightPoint, Vector2.down * raycastLength, _isGrounded ? Color.green : Color.red);
        }

        #region INPUT

        private void MovementPerformed(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
            _horizontalInput = _moveInput.x;
        }
        
        private void MovementCanceled(InputAction.CallbackContext ctx)
        {
            _moveInput = Vector2.zero;
            _horizontalInput = _moveInput.x;
        }
        
        public void SetHorizontalInput(float horizontalInput)
        {
            _horizontalInput = horizontalInput;
        }

        #endregion

        #region MOVEMENT
        public void Jump()
        {
            if (!_isGrounded)
                return;
            _isJumping = true;
            _startJumpHeight = transform.position.y;
            _currentGravity = 0;
            // Calculate the initial vertical speed for the jump
            _verticalSpeed = Mathf.Sqrt(2 * gravity * maxJumpHeight);
            OnJump?.Invoke();
            StartCoroutine(VerticalImpulseRoutine(jumpImpulseMultiplier, jumpImpulseDuration)); // Start the easing out coroutine
            _animationController.UpdateMovementState(CharacterMovementState.JUMP_LAUNCH);
        }

        private void Move()
        {
            // VERTICAL MOVEMENT
            // Apply vertical speed (jumping or falling)
            float verticalVelocity =
                _verticalSpeed * Time.fixedDeltaTime * _verticalImpulseMultiplier*_flowerBoostImpulse + _boostExtraVelocity;
            transform.position += new Vector3(0f, verticalVelocity, 0f);

            if (transform.position.y >= 0.8f * (_startJumpHeight + maxJumpHeight*_flowerBoostHeightMultiplier) + _extraHeight)
            {
                _currentGravity = 4 * gravity;
            }
            

            if (!_isGrounded)
            {
                // HORIZONTAL MOVEMENT
                transform.position += new Vector3(_horizontalInput * Time.fixedDeltaTime * horizontalSpeed * _horizontalImpulseMultiplier, 0f, 0f);
                FlipCharacterScale(_horizontalInput);
            }
        }
        private void HandleFall()
        {
            if (!_isFalling)
                return;
        }

        public void Boost(float boostExtraVelocity, float extraHeight, float boostDuration)
        {
            _boostExtraVelocity += boostExtraVelocity;
            _extraHeight += extraHeight;
            StartCoroutine(BoostRoutine(boostDuration, boostExtraVelocity));
        }

        public void FlowerBoost(float boostImpulse, float heightMultiplier, float boostDuration, GameObject flower)
        {
            if (!_isFalling)
                return;
            _isGrounded = true;
            _isFalling = false;
            _fallStart = false;
            _flowerBoostHeightMultiplier = heightMultiplier;
            Jump();
            _isGrounded = false;
            StartCoroutine(FlowerBoostImpulseRoutine(boostDuration, boostImpulse));
        }
        
        private IEnumerator VerticalImpulseRoutine(float targetMultiplier, float duration)
        {
            float time = 0f;

            // Start from the targetMultiplier and gradually lerp to 1
            while (time < duration)
            {
                _verticalImpulseMultiplier = Mathf.Lerp(targetMultiplier, 1, time / duration);
                time += Time.deltaTime;
                yield return null; // Wait until next frame
            }

            _verticalImpulseMultiplier = 1f; // Ensure the impulseMultiplier is set to 1 after the duration
        }
        private IEnumerator HorizontalImpulseRoutine(float targetMultiplier, float duration)
        {
            float time = 0f;

            // Start from the targetMultiplier and gradually lerp to 1
            while (time < duration)
            {
                _horizontalImpulseMultiplier = Mathf.Lerp(targetMultiplier, 1, time / duration);
                time += Time.deltaTime;
                yield return null; // Wait until next frame
            }

            _horizontalImpulseMultiplier = 1f; // Ensure the impulseMultiplier is set to 1 after the duration
        }
        private IEnumerator BoostRoutine(float duration, float boostExtraVelocity)
        {
            float time = 0f;
            while (time < duration)
            {
                _boostExtraVelocity = Mathf.Lerp(boostExtraVelocity, 0f, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            
            _boostExtraVelocity = 0f;
            _extraHeight = 0f;
        }
        private IEnumerator FlowerBoostImpulseRoutine(float duration, float boostImpulse)
        {
            float time = 0f;
            while (time < duration)
            {
                _flowerBoostImpulse = Mathf.Lerp(boostImpulse, 1f, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            
            _flowerBoostImpulse = 1f;
            _flowerBoostHeightMultiplier = 1f;
        }
        private void FlipCharacterScale(float horizontalInput)
        {
            // Flip the character's scale only when moving left or right
            if (horizontalInput < 0)
            {
                // Flip scale for left movement
                transform.localScale = new Vector3(-Mathf.Abs(_originalScale.x), _originalScale.y, _originalScale.z);
            }
            else if (horizontalInput > 0)
            {
                // Reset to original scale for right movement
                transform.localScale = _originalScale;
            }
            // When horizontalInput is 0, you can choose to keep the character's current direction or reset it.
        }
        #endregion
    }
}
