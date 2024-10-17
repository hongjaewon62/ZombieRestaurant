using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCounter : MonoBehaviour
{
    public bool isActive;

    [SerializeField]
    private BoxCollider destinationCollider;

    private void Awake()
    {
        destinationCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        isActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isActive = false;
    }
}
