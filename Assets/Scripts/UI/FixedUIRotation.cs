using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedUIRotation : MonoBehaviour
{
    [SerializeField]
    private Vector3 vector;
    private void Update()
    {
        transform.rotation = Quaternion.Euler(vector.x, vector.y, vector.z);
    }
}
