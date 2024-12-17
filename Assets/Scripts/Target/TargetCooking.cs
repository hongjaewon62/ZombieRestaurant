using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCooking : MonoBehaviour
{
    public Menu menu;

    public bool isActive;
    public float cookingTime;

    private void Start()
    {
        cookingTime = menu.cookingTime;
    }

    private void OnEnable()
    {
        cookingTime = menu.cookingTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Zombie"))
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
