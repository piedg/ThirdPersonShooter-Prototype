using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : PlayerBaseState
{
    readonly private int PistolAimLocomotionHash = Animator.StringToHash("PistolAimLocomotion");

    readonly private int SpeedHash = Animator.StringToHash("Speed");
    readonly private int MoveYHash = Animator.StringToHash("MoveY");

    private const float AnimatorDampTime = 0.1f;

    private float currentSpeed;

    public PlayerAimState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(PistolAimLocomotionHash, 0.1f);
        stateMachine.AimVirtualCamera.gameObject.SetActive(true);
    }
    public override void Tick(float deltaTime)
    {
        if (!stateMachine.InputManager.IsAiming)
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            return;
        }

        Vector3 movement = CalculateMovement();
        currentSpeed = stateMachine.InputManager.IsSprinting ? stateMachine.SprintSpeed : stateMachine.NormalSpeed;

        Move(movement * currentSpeed, deltaTime);

        stateMachine.Animator.SetFloat(MoveYHash, stateMachine.InputManager.MovementValue.y, AnimatorDampTime, deltaTime);
      

        if (stateMachine.InputManager.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(MoveYHash, 0, AnimatorDampTime, deltaTime);
            return;
        }


        // Build a LookCamera method for VirtualCamera

    }

    public override void Exit()
    {
        stateMachine.AimVirtualCamera.gameObject.SetActive(false);
    }
}
