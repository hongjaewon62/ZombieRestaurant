using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetOrder : MonoBehaviour
{
    public bool isActive ;
    public bool isFood;
    private TargetCounter targetCounter;
    private ZombieController zombie;

    private void Awake()
    {
        targetCounter = gameObject.transform.parent.GetComponent<TargetCounter>();
    }

    private void Update()
    {
        if(isFood)
        {
            targetCounter.isFood = true;
        }
        else
        {
            targetCounter.isFood = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Zombie"))
        {
            zombie = other.GetComponent<ZombieController>();
        }

        if(other.CompareTag("Zombie") && targetCounter.isReady)
        {
            isActive = true;
        }

        if(other.CompareTag("Zombie") && zombie.isFood)
        {
            // 음식 전달
            targetCounter.isReady = false;
            zombie.GiveFood();
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
