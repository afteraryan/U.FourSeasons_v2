using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Player_Character.Script
{
    public class Depriciated2_CharacterController : MonoBehaviour
    {
        [SerializeField] private float jumpVelocity;
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
        
        //INPUT
        private InputSystem _playerInput;
        private Vector2 _moveInput;
        private float _horizontalInput;
        
        private void Awake()
        {
            _playerInput = new InputSystem();
            _currentGravity = gravity;
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
                    _currentGravity = 2 * gravity;
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

        #endregion

        #region MOVEMENT
        private void Jump()
        {
            if (_isGrounded)
            {
                _isJumping = true;
                _startJumpHeight = transform.position.y;
                _currentGravity = 0;
                // Calculate the initial vertical speed for the jump
                _verticalSpeed = Mathf.Sqrt(2 * gravity * maxJumpHeight);
                StartCoroutine(VerticalImpulseRoutine(jumpImpulseMultiplier, jumpImpulseDuration)); // Start the easing out coroutine
            }
        }

        private void Move()
        {
            // VERTICAL MOVEMENT
            // Apply vertical speed (jumping or falling)
            transform.position += new Vector3(0f, _verticalSpeed * Time.fixedDeltaTime * _verticalImpulseMultiplier, 0f);

            if (transform.position.y >= 0.8f * (_startJumpHeight + maxJumpHeight))
            {
                _currentGravity = 3 * gravity;
            }
            

            if (!_isGrounded)
            {
                // HORIZONTAL MOVEMENT
                transform.position += new Vector3(_horizontalInput * Time.fixedDeltaTime * _horizontalImpulseMultiplier, 0f, 0f);
            }
        }
        private void HandleFall()
        {
            if (!_isFalling)
                return;
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
        #endregion
    }
}
