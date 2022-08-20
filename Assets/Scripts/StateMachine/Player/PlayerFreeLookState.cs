using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeLookState : PlayerBaseState 
{ 
    readonly private int FreeLookLocomotionHash = Animator.StringToHash("FreeLookLocomotion");
    readonly private int StandingToCrouchHash = Animator.StringToHash("StandingToCrouch");
    readonly private  int FreeLookSpeedHash = Animator.StringToHash("Speed");
    private const float AnimatorDampTime = 0.1f;

    private float currentSpeed;

    public PlayerFreeLookState(PlayerStateMachine stateMachine) : base(stateMachine) { }
 
    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(FreeLookLocomotionHash, 0.3f);
        stateMachine.InputManager.JumpEvent += OnJump;
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.InputManager.IsCrouching)
        {
            stateMachine.Animator.CrossFadeInFixedTime(StandingToCrouchHash, 0.1f);
            stateMachine.SwitchState(new PlayerCrouchingState(stateMachine));
            return;
        }

        Vector3 movement = CalculateMovement();
        currentSpeed = stateMachine.InputManager.IsSprinting ? stateMachine.SprintSpeed : stateMachine.NormalSpeed;
        
        Move(movement * currentSpeed, deltaTime);

        if (stateMachine.InputManager.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }

        stateMachine.Animator.SetFloat(FreeLookSpeedHash, currentSpeed, AnimatorDampTime, deltaTime);
        FaceMovementDirection(movement, deltaTime);
    }

    public override void Exit() 
    {
        stateMachine.InputManager.JumpEvent -= OnJump;
    }

    void OnJump()
    {
        stateMachine.SwitchState(new PlayerJumpingState(stateMachine));
    }
}
