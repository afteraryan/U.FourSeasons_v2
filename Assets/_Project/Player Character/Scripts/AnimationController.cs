using _Project.Player_Character.Scripts;
using UnityEngine;

public class AnimationController
{
    private Animator _animator;
    private CharacterMovementState _currentMovementState;
    private static readonly int TriggerIdle = Animator.StringToHash("trigger_Idle");
    private static readonly int TriggerJumpPrepare = Animator.StringToHash("trigger_Jump_Prepare");
    private static readonly int TriggerJumpLaunch = Animator.StringToHash("trigger_Jump_Launch");
    private static readonly int TriggerJumpFall = Animator.StringToHash("trigger_Jump_Fall");
    private static readonly int TriggerJumpLand = Animator.StringToHash("trigger_Jump_Land");

    public void Init(Animator animator)
    {
        _animator = animator;
    }

    public void UpdateMovementState(CharacterMovementState state)
    {
        switch (state)
        {
            case CharacterMovementState.IDLE:
                _currentMovementState = CharacterMovementState.IDLE;
                break;
            case CharacterMovementState.JUMP_PREPARE:
                _currentMovementState = CharacterMovementState.JUMP_PREPARE;
                break;
            case CharacterMovementState.JUMP_LAUNCH:
                _currentMovementState = CharacterMovementState.JUMP_LAUNCH;
                break;
            case CharacterMovementState.JUMP_FALL:
                _currentMovementState = CharacterMovementState.JUMP_FALL;
                break;
            case CharacterMovementState.JUMP_LAND:
                _currentMovementState = CharacterMovementState.JUMP_LAND;
                break;
            default:
                _currentMovementState = CharacterMovementState.IDLE;
                break;
        }
        UpdateAnimation();
    }
    
    private void UpdateAnimation()
    {
        switch (_currentMovementState)
        {
            case CharacterMovementState.IDLE:
                _animator.SetTrigger(TriggerIdle);
                break;
            case CharacterMovementState.JUMP_PREPARE:
                _animator.SetTrigger(TriggerJumpPrepare);
                break;
            case CharacterMovementState.JUMP_LAUNCH:
                _animator.SetTrigger(TriggerJumpLaunch);
                break;
            case CharacterMovementState.JUMP_FALL:
                _animator.SetTrigger(TriggerJumpFall);
                break;
            case CharacterMovementState.JUMP_LAND:
                _animator.SetTrigger(TriggerJumpLand);
                break;
            default:
                break;
        }
    }
}
