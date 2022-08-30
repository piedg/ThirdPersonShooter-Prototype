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

    protected Vector3 CalculateAimMovement()
    {
        return stateMachine.MainCameraTransform.transform.eulerAngles += 5 * new Vector3(stateMachine.InputManager.LookValue.y, stateMachine.InputManager.LookValue.x, 0f);
    }

    protected void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            Quaternion.LookRotation(movement),
            deltaTime * stateMachine.RotationSpeed);
    }
}
