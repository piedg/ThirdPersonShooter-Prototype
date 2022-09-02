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

    public Vector3 MouseWorldPosition;

    public PlayerAimState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.CinemachineTargetYaw = stateMachine.CinemachineCameraTarget.transform.rotation.eulerAngles.y;

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
        AimCameraRotation(deltaTime);

        Vector3 movement = CalculateMovement();
        currentSpeed = stateMachine.InputManager.IsSprinting ? stateMachine.SprintSpeed : stateMachine.NormalSpeed;

        PlayerAimRotation();

        Move(movement * currentSpeed, deltaTime);

        #region Animator

        stateMachine.Animator.SetFloat(MoveYHash, stateMachine.InputManager.MovementValue.y, AnimatorDampTime, deltaTime);
        stateMachine.Animator.SetFloat(MoveXHash, stateMachine.InputManager.MovementValue.x, AnimatorDampTime, deltaTime);

        #endregion

        if (stateMachine.InputManager.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(MoveYHash, 0, AnimatorDampTime, deltaTime);
            stateMachine.Animator.SetFloat(MoveXHash, 0, AnimatorDampTime, deltaTime);
            PlayerAimRotation();
            AimCameraRotation(deltaTime);
            return;
        }


    }

    public override void Exit()
    {
        stateMachine.AimVirtualCamera.gameObject.SetActive(false);
        stateMachine.CrossHair.gameObject.SetActive(false);
    }

    void PlayerAimRotation()
    {
        MouseWorldPosition = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(stateMachine.CrossHair.transform.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, stateMachine.AimColliderLayerMask))
        {
            MouseWorldPosition = hit.point;
        }

        Vector3 worldAimTarget = MouseWorldPosition;
        worldAimTarget.y = stateMachine.transform.position.y;

        Vector3 aimDirection = (worldAimTarget - stateMachine.transform.position).normalized;

        stateMachine.transform.forward = Vector3.Lerp(stateMachine.transform.forward, aimDirection, Time.deltaTime * 20f);
    }
}
