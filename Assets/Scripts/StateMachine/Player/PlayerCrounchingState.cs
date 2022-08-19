using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrounchingState : PlayerBaseState
{
    readonly private int CrouchingToStandHash = Animator.StringToHash("CrouchingToStand");
    readonly private int CrouchingLocomotionHash = Animator.StringToHash("CrouchingLocomotion");
    readonly private int CrouchSpeedHash = Animator.StringToHash("CrouchSpeed");
    private float AnimatorDampTime = 0.1f;
    public PlayerCrounchingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Enter Crouching");
        stateMachine.Animator.CrossFadeInFixedTime(CrouchingLocomotionHash, 0.5f);
    }

    public override void Tick(float deltaTime)
    {
        Vector3 movement = CalculateMovement();

        Move(movement * stateMachine.CrouchSpeed, deltaTime);

        if (stateMachine.InputManager.IsSprinting)
        {
            stateMachine.Animator.CrossFadeInFixedTime(CrouchingToStandHash, 0.1f);
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            return;
        }
        if (stateMachine.InputManager.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(CrouchSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }

        stateMachine.Animator.SetFloat(CrouchSpeedHash, 1f, AnimatorDampTime, deltaTime);
        FaceMovementDirection(movement, deltaTime);
    }

    public override void Exit() { }

}
