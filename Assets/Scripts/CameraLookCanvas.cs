using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookCanvas : MonoBehaviour
{
    [SerializeField]
    private GameObject camera;

    private void LateUpdate()
    {
        if (camera != null)
        {
            transform.LookAt(camera.transform);
            transform.Rotate(0, 180, 0);
        }
    }
}
