using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrounchingState : PlayerBaseState
{
    readonly private int CrouchingToStandHash = Animator.StringToHash("CrouchingToStand");
    readonly private int CrouchingLocomotionHash = Animator.StringToHash("CrouchingLocomotion");
    readonly private int CrouchSpeedHash = Animator.StringToHash("CrouchSpeed");
    public PlayerCrounchingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(CrouchingLocomotionHash, 0.5f);
        Debug.Log("Crouching");
    }

    public override void Exit()
    {
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.Animator.SetFloat(CrouchSpeedHash, 0, 0.1f, deltaTime);

        if (stateMachine.InputManager.IsSprinting)
        {
            stateMachine.Animator.CrossFadeInFixedTime(CrouchingToStandHash, 0.1f);
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            return;
        }
    }
}
