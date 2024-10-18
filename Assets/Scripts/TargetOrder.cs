using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetOrder : MonoBehaviour
{
    public bool isActive ;
    private TargetCounter targetCounter;

    private void Awake()
    {
        targetCounter = gameObject.transform.parent.GetComponent<TargetCounter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Zombie") && targetCounter.isReady)
        {
            isActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            isActive = false;
        }
    }
}
