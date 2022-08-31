using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : PlayerBaseState
{
    readonly private int PistolAimLocomotionHash = Animator.StringToHash("PistolAimLocomotion");
    readonly private int MoveYHash = Animator.StringToHash("MoveY");
    readonly private int MoveXHash = Animator.StringToHash("MoveX");

    private const float AnimatorDampTime = 0.1f;

    private float currentSpeed;

    public PlayerAimState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(PistolAimLocomotionHash, 0.1f);
        stateMachine.AimVirtualCamera.gameObject.SetActive(true);
        stateMachine.CrossHair.gameObject.SetActive(true);
    }

    public override void Tick(float deltaTime)
    {
        if (!stateMachine.InputManager.IsAiming)
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            return;
        }

        CameraRotation(deltaTime);

        Vector3 movement = CalculateMovement();
        currentSpeed = stateMachine.InputManager.IsSprinting ? stateMachine.SprintSpeed : stateMachine.NormalSpeed;

        Move(movement * currentSpeed, deltaTime);

        #region Animator

        stateMachine.Animator.SetFloat(MoveYHash, stateMachine.InputManager.MovementValue.y, AnimatorDampTime, deltaTime);
        stateMachine.Animator.SetFloat(MoveXHash, stateMachine.InputManager.MovementValue.x, AnimatorDampTime, deltaTime);

        #endregion

        if (stateMachine.InputManager.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(MoveYHash, 0, AnimatorDampTime, deltaTime);
            stateMachine.Animator.SetFloat(MoveXHash, 0, AnimatorDampTime, deltaTime);
            CameraRotation(deltaTime);
            return;
        }
    }

    public override void Exit()
    {
        stateMachine.AimVirtualCamera.gameObject.SetActive(false);
        stateMachine.CrossHair.gameObject.SetActive(false);
    }
    
    private void CameraRotation(float deltaTime)
    {

        if (stateMachine.InputManager.LookValue.sqrMagnitude >= 0.01f )
        {
           stateMachine.CinemachineTargetYaw += stateMachine.InputManager.LookValue.x * deltaTime * stateMachine.AimSensitivity;
            stateMachine.CinemachineTargetPitch += stateMachine.InputManager.LookValue.y * deltaTime * stateMachine.AimSensitivity; 
        }

        // Limita rotazione della camera
        stateMachine.CinemachineTargetYaw = ClampAngle(stateMachine.CinemachineTargetYaw, float.MinValue, float.MaxValue);
        stateMachine.CinemachineTargetPitch = ClampAngle(stateMachine.CinemachineTargetPitch, stateMachine.BottomClamp, stateMachine.TopClamp);

        // Cinemachine will follow this target
        stateMachine.CinemachineCameraTarget.transform.rotation = Quaternion.Euler(stateMachine.CinemachineTargetPitch,
            stateMachine.CinemachineTargetYaw, 0.0f);
    }
}
