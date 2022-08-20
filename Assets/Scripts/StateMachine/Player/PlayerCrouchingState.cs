using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchingState : PlayerBaseState
{
    readonly private int CrouchingToStandHash = Animator.StringToHash("CrouchingToStand");
    readonly private int CrouchingLocomotionHash = Animator.StringToHash("CrouchingLocomotion");
    readonly private int CrouchSpeedHash = Animator.StringToHash("CrouchSpeed");
    private float AnimatorDampTime = 0.1f;

    Vector3 ControllerCenterOnCrouch = new Vector3(0f, 0.65f, 0f);
    float ControllerHeightOnCrouch = 1.5f;

    public PlayerCrouchingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Controller.height = ControllerHeightOnCrouch;
        stateMachine.Controller.center = ControllerCenterOnCrouch;

        stateMachine.Animator.CrossFadeInFixedTime(CrouchingLocomotionHash, 0.3f);
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.InputManager.IsSprinting)
        {
            stateMachine.Animator.CrossFadeInFixedTime(CrouchingToStandHash, 0.1f);
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            return;
        }

        Vector3 movement = CalculateMovement();

        Move(movement * stateMachine.CrouchSpeed, deltaTime);

        if (stateMachine.InputManager.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(CrouchSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }

        stateMachine.Animator.SetFloat(CrouchSpeedHash, 1f, AnimatorDampTime, deltaTime);
        FaceMovementDirection(movement, deltaTime);
    }

    public override void Exit() 
    {
        stateMachine.Controller.height = stateMachine.ControllerHeight;
        stateMachine.Controller.center = stateMachine.ControllerCenter;
    }
}
