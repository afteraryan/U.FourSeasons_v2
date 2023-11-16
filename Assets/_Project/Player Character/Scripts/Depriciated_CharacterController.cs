using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Player_Character.Script
{
    public class Depriciated_CharacterController : MonoBehaviour
    {
        [SerializeField] private float velocity;
        [SerializeField] private float gravity;
        [SerializeField] private float singleJumpDistance;
        private float _remainingJumpDistance;
        private float _currentJumpDistance;
        
        [Header("Ground Check Parameters")]
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private LayerMask whatIsGround;
        [Header("Raycast Ground Check Parameters")]
        [SerializeField] private float raycastLength = 0.3f; // Length of the raycasts
        [SerializeField] private float raycastSideOffset = 0.5f; // Offset from the center for the side raycasts
        
        //JUMP
        private bool _isInAir;
        private bool _isJumping;
        private bool _isRising;
        
        //INPUT
        private InputSystem _inputSystem;
        private float _horizontalInput;
        
        

        private void Awake()
        {
            _inputSystem = new InputSystem();
        }
        private void OnEnable()
        {
            _inputSystem.Enable();
            _inputSystem.PlayerCharacter2D.Movement.performed += ctx => HandleInput(); 
            _inputSystem.PlayerCharacter2D.Movement.canceled += ctx => _horizontalInput = 0f;
            _inputSystem.PlayerCharacter2D.Jump.performed += ctx => JumpPressed();
            _inputSystem.PlayerCharacter2D.Jump.canceled += ctx => JumpReleased();
        }
        private void OnDisable()
        {
            _inputSystem.Disable();
            _inputSystem.PlayerCharacter2D.Movement.performed -= ctx => HandleInput(); 
            _inputSystem.PlayerCharacter2D.Movement.canceled -= ctx => _horizontalInput = 0f;
            _inputSystem.PlayerCharacter2D.Jump.performed -= ctx => JumpPressed();
            _inputSystem.PlayerCharacter2D.Jump.canceled -= ctx => JumpReleased();
        }

        private void Update()
        {
            CheckIfInAir();
        }

        private void FixedUpdate()
        {
            Move();
        }


        #region INPUTHANDLING
        
        private void HandleInput()
        {
            _horizontalInput = _inputSystem.PlayerCharacter2D.Movement.ReadValue<float>();
        }
        
        private void JumpPressed()
        {
            if (_isInAir)
                return;
            
            //transform.localScale = Vector3.one*0.8f;
            
        }

        private void JumpReleased()
        {
            if (_isInAir)
                return;
            Jump();
        }

        #endregion

        #region MOVEMENT

        private void CheckIfInAir()//Hawa main hai kya ?
        {
            Vector2 centerPoint = groundCheckPoint.position;
            Vector2 leftPoint = centerPoint + Vector2.left * raycastSideOffset;
            Vector2 rightPoint = centerPoint + Vector2.right * raycastSideOffset;
            bool centerHit = Physics2D.Raycast(centerPoint, Vector2.down, raycastLength, whatIsGround);
            bool leftHit = Physics2D.Raycast(leftPoint, Vector2.down, raycastLength, whatIsGround);
            bool rightHit = Physics2D.Raycast(rightPoint, Vector2.down, raycastLength, whatIsGround);

            // Debug rays
            Color centerColor = centerHit ? Color.green : Color.red;
            Color leftColor = leftHit ? Color.green : Color.red;
            Color rightColor = rightHit ? Color.green : Color.red;
            Debug.DrawRay(centerPoint, Vector2.down * raycastLength, centerColor);
            Debug.DrawRay(leftPoint, Vector2.down * raycastLength, leftColor);
            Debug.DrawRay(rightPoint, Vector2.down * raycastLength, rightColor);
        
            _isInAir = !(centerHit || leftHit || rightHit);
            _isJumping = _isInAir? _isJumping: false;
        }
        

        private void Jump()
        {
            transform.localScale = Vector3.one;
            _isJumping = true;
            _isRising = true;

            _currentJumpDistance = singleJumpDistance;
            _remainingJumpDistance = 0f;
        }

        private void Move()
        {
            if (_isRising && _remainingJumpDistance < _currentJumpDistance)
            {
                _remainingJumpDistance += velocity * Time.fixedDeltaTime;
                MoveUp();
            }
            else if(_remainingJumpDistance >= _currentJumpDistance)
            {
                _isRising = false;
                _remainingJumpDistance = 0f;
                _currentJumpDistance = 0f;
            }
            
        }

        private void MoveUp()
        {
            transform.position += new Vector3(0f,velocity * Time.fixedDeltaTime,0f);
        }

        #endregion
        
    }
}