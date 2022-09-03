using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithMouse : MonoBehaviour
{
    public float speed = 5;
    public InputManager InputManager;
    public GameObject MainCamera;

    private void LateUpdate()
    {
        Debug.Log("Sono qui");
        MainCamera.transform.LookAt(new Vector3(InputManager.LookValue.y, InputManager.LookValue.x, 0f));
        Debug.Log(InputManager.LookValue.y);
    }
}
