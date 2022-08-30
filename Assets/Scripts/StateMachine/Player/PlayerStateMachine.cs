using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerStateMachine : StateMachine
{
    [field: SerializeField, Header("Components")] public CharacterController Controller { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public InputManager InputManager { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField, Header("Movement Settings")] public float NormalSpeed { get; private set; }
    [field: SerializeField] public float SprintSpeed {get; private set; }
    [field: SerializeField] public float CrouchSpeed {get; private set; }
    [field: SerializeField] public float RotationSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }

    [field: SerializeField, Header("Aim Settings")] public CinemachineVirtualCamera AimVirtualCamera { get; private set; }
    [field: SerializeField] public GameObject CrossHair;
    [field: SerializeField] public LayerMask AimColliderLayerMask = new LayerMask();
    [field: SerializeField] public GameObject CinemachineCameraTarget;
    [field: SerializeField] public float AimSensitivity = 1f;

    // Character Controller Shape
    public float ControllerHeight { get; private set; }
    public Vector3 ControllerCenter { get; private set; }

    public Transform MainCameraTransform { get; private set; }

    private void Start()
    {
        ControllerHeight = Controller.height;
        ControllerCenter = Controller.center;

        MainCameraTransform = Camera.main.transform;
        SwitchState(new PlayerFreeLookState(this));
    }
}
