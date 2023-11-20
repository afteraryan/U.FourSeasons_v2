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
        
        [Header("PHYSICS PARAMETERS")]
        [SerializeField] private CharacterPhysicsParameters initialPhysicsParameters;
        [SerializeField] private CharacterPhysicsParameters normalPhysicsParameters;

        #region PHYSICS PARAMETERS
        private float gravity;
        private float jumpVelocity;
        private float horizontalSpeed;
        private float maxJumpHeight;
        private float jumpImpulseMultiplier = 1f;
        private float jumpImpulseDuration = 0f;
        private float fallImpulseMultiplier = 1f;
        private float fallImpulseDuration = 0f;
        private float fallForce;
        private CharacterPhysicsParameters _currentPhysicsParameters;
        #endregion
        
        private float _currentGravity;
        private float _verticalSpeed;
        private float _verticalImpulseMultiplier = 1f;
        private float _horizontalImpulseMultiplier = 1f;
        private float _boostExtraVelocity;
        private float _flowerBoostImpulse = 1f;
        private float _flowerBoostHeightMultiplier = 1f;
        private float _currentFallForce = 1;
        private float _jumpHeight;
        
        
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
        private bool _isFalling = false;
        private bool _fallStart = false;
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
        
        public static event Action PlayerDied;

        #region EVENT FUNCTIONS
        private void Awake()
        {
            SetPhysicsParameters(initialPhysicsParameters);
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
            _playerInput.PlayerCharacter2D.Jump.performed += OnJumpPerformed;
        }
        private void OnDisable()
        {
            _playerInput.Disable();
            _playerInput.PlayerCharacter2D.Movement.performed -= MovementPerformed;
            _playerInput.PlayerCharacter2D.Movement.canceled -= MovementCanceled;
            _playerInput.PlayerCharacter2D.Jump.performed -= OnJumpPerformed;
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
                    _extraHeight = 0;
                    OnLand?.Invoke();
                    //_currentGravity = 0;
                    _currentFallForce = 1;
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
                    _currentFallForce = fallForce;
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
        #endregion

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

        public void Die()
        {
            Debug.Log("Player died");

            // Make character fall
            StartCoroutine(FallAndDieRoutine());
        }
        private IEnumerator FallAndDieRoutine()
        {
            // Adjust gravity to make the character fall
            _currentGravity = -gravity; // Assuming negative gravity will make the character fall

            // Wait for 2 seconds
            yield return new WaitForSeconds(0.5f);

            // Trigger the PlayerDied event
            PlayerDied?.Invoke();
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
        
        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            Jump();
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
                _verticalSpeed * Time.fixedDeltaTime * _verticalImpulseMultiplier*_flowerBoostImpulse*fallForce*_currentFallForce + _boostExtraVelocity;
            transform.position += new Vector3(0f, verticalVelocity, 0f);

            _jumpHeight = 0.8f * (_startJumpHeight + maxJumpHeight * _flowerBoostHeightMultiplier) + _extraHeight;

            if (transform.position.y >= _jumpHeight)
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

        
        Coroutine _boostCoroutine;
        public void Boost(float boostExtraVelocity, float extraHeight, float boostDuration)
        {
            if(_boostCoroutine != null)
                StopCoroutine(_boostCoroutine);
            _boostExtraVelocity += boostExtraVelocity;
            _extraHeight += extraHeight;
            _boostCoroutine = StartCoroutine(BoostRoutine(boostDuration, boostExtraVelocity));
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

        #region Wearables

        public void WearJumpyShoes()
        {
            SetPhysicsParameters(normalPhysicsParameters);
        }

        #endregion

        #region UTILITIES

        public bool GetIsFalling()
        {
            return _isFalling;
        }
        
        private void SetPhysicsParameters(CharacterPhysicsParameters physicsParameters)
        {
            gravity = physicsParameters.gravity;
            jumpVelocity = physicsParameters.jumpVelocity;
            horizontalSpeed = physicsParameters.horizontalSpeed;
            maxJumpHeight = physicsParameters.maxJumpHeight;
            jumpImpulseMultiplier = physicsParameters.jumpImpulseMultiplier;
            jumpImpulseDuration = physicsParameters.jumpImpulseDuration;
            fallImpulseMultiplier = physicsParameters.fallImpulseMultiplier;
            fallImpulseDuration = physicsParameters.fallImpulseDuration;
            fallForce = physicsParameters.fallForce;
            _currentPhysicsParameters = physicsParameters;
        }

        #endregion
    }
}

[Serializable]
public struct CharacterPhysicsParameters
{
    public float gravity;
    public float jumpVelocity;
    public float horizontalSpeed;
    public float maxJumpHeight;
    public float jumpImpulseMultiplier;
    public float jumpImpulseDuration;
    public float fallImpulseMultiplier;
    public float fallImpulseDuration;
    public float fallForce;
}

