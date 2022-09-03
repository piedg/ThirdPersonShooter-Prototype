using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected PlayerStateMachine stateMachine;

    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void Move(float deltaTime)
    {
        // Handle player no movement input
        Move(Vector3.zero, deltaTime);
    }

    protected void Move(Vector3 direction, float deltaTime)
    {
        stateMachine.Controller.Move((direction + stateMachine.ForceReceiver.Movement) * deltaTime);
    }

    protected Vector3 CalculateMovement()
    {
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward * stateMachine.InputManager.MovementValue.y + right * stateMachine.InputManager.MovementValue.x;
    }

    protected void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            Quaternion.LookRotation(movement),
            deltaTime * stateMachine.RotationSpeed);
    }

    protected void AimCameraRotation(float deltaTime)
    {
        if (stateMachine.InputManager.LookValue.sqrMagnitude >= 0.01f)
        {
            stateMachine.CinemachineTargetYaw += stateMachine.InputManager.LookValue.x * deltaTime * stateMachine.AimSensitivity;
            stateMachine.CinemachineTargetPitch -= stateMachine.InputManager.LookValue.y * deltaTime * stateMachine.AimSensitivity;
        }

        // Limita rotazione della camera
        stateMachine.CinemachineTargetYaw = ClampAngle(stateMachine.CinemachineTargetYaw, float.MinValue, float.MaxValue);
        stateMachine.CinemachineTargetPitch = ClampAngle(stateMachine.CinemachineTargetPitch, stateMachine.BottomClamp, stateMachine.TopClamp);


        // Cinemachine will follow this target
        stateMachine.CinemachineCameraTarget.transform.rotation = Quaternion.Euler(stateMachine.CinemachineTargetPitch,
            stateMachine.CinemachineTargetYaw, 0.0f);
    }

    protected float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
