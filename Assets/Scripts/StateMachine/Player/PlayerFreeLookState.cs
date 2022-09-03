using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeLookState : PlayerBaseState 
{ 
    readonly private int FreeLookLocomotionHash = Animator.StringToHash("FreeLookLocomotion");
    readonly private  int SpeedHash = Animator.StringToHash("Speed");
    private const float AnimatorDampTime = 0.1f;

    private float currentSpeed;

    public PlayerFreeLookState(PlayerStateMachine stateMachine) : base(stateMachine) { }
 
    public override void Enter()
    {
        stateMachine.InputManager.JumpEvent += OnJump;
        stateMachine.InputManager.CrouchEvent += OnCrouch;

        stateMachine.Animator.CrossFadeInFixedTime(FreeLookLocomotionHash, 0.3f);
    }

    public override void Tick(float deltaTime)
    {
        if(stateMachine.InputManager.IsAiming)
        {
            stateMachine.SwitchState(new PlayerAimState(stateMachine));
            return;
        }

        Vector3 movement = CalculateMovement();
        currentSpeed = stateMachine.InputManager.IsSprinting ? stateMachine.SprintSpeed : stateMachine.NormalSpeed;
        
        Move(movement * currentSpeed, deltaTime);

        if (stateMachine.InputManager.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(SpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }

        stateMachine.Animator.SetFloat(SpeedHash, currentSpeed, AnimatorDampTime, deltaTime);
        FaceMovementDirection(movement, deltaTime);
    }

    public override void Exit() 
    {
        stateMachine.InputManager.JumpEvent -= OnJump;
        stateMachine.InputManager.CrouchEvent -= OnCrouch;
    }

    void OnJump()
    {
        stateMachine.SwitchState(new PlayerJumpingState(stateMachine));
    }

    void OnCrouch()
    {
        stateMachine.SwitchState(new PlayerCrouchingState(stateMachine));
    }
}
