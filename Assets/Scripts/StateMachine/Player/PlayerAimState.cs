using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : PlayerBaseState
{
    readonly private int PistolAimLocomotionHash = Animator.StringToHash("PistolAimLocomotion");

    readonly private int SpeedHash = Animator.StringToHash("Speed");
    readonly private int MoveYHash = Animator.StringToHash("MoveY");
    readonly private int MoveXHash = Animator.StringToHash("MoveX");

    private const float AnimatorDampTime = 0.1f;

    private float currentSpeed;
    private Vector3 mouseWorldPosition;

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

        CameraRotation();

        Vector3 movement = CalculateMovement();
        currentSpeed = stateMachine.InputManager.IsSprinting ? stateMachine.SprintSpeed : stateMachine.NormalSpeed;


        Aim(deltaTime);
        //FaceMovementDirection(movement, deltaTime);
        Move(movement * currentSpeed, deltaTime);


        stateMachine.Animator.SetFloat(MoveYHash, stateMachine.InputManager.MovementValue.y, AnimatorDampTime, deltaTime);

        stateMachine.Animator.SetFloat(MoveXHash, stateMachine.InputManager.MovementValue.x, AnimatorDampTime, deltaTime);


        if (stateMachine.InputManager.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(MoveYHash, 0, AnimatorDampTime, deltaTime);
            stateMachine.Animator.SetFloat(MoveXHash, 0, AnimatorDampTime, deltaTime);
            CameraRotation();

            return;
        }

    }

    public override void Exit()
    {
        stateMachine.AimVirtualCamera.gameObject.SetActive(false);
        stateMachine.CrossHair.gameObject.SetActive(false);
    }

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (stateMachine.InputManager.LookValue.sqrMagnitude >= 0.01f )
        {
            _cinemachineTargetYaw += stateMachine.InputManager.LookValue.x * Time.deltaTime * stateMachine.AimSensitivity;
            _cinemachineTargetPitch += stateMachine.InputManager.LookValue.y * Time.deltaTime * stateMachine.AimSensitivity;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        stateMachine.CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void Aim(float deltaTime)
    {
        Vector3 mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, stateMachine.AimColliderLayerMask))
        {
            stateMachine.transform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = stateMachine.transform.position.y;
        Vector3 aimDiretcion = (worldAimTarget - stateMachine.transform.position).normalized;
     
            stateMachine.transform.rotation = Quaternion.Lerp(
                stateMachine.transform.rotation,
                Quaternion.LookRotation(mouseWorldPosition),
                deltaTime * stateMachine.RotationSpeed);
        // stateMachine.transform.forward = Vector3.Lerp(stateMachine.transform.forward, aimDiretcion, deltaTime * 20f);

    }
}
