using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCounter : MonoBehaviour
{
    public bool isActive;
    public bool isReady;

    public event Action<TargetCounter> OnReadyChanged; // 이벤트 선언

    private GuestController guest;


    private void Awake()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        isActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isActive = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Guest") && isActive)
        {
            guest = other.GetComponent<GuestController>();
            if (!isReady && guest.isReady)
            {
                isReady = true;
                OnReadyChanged?.Invoke(this);       // 이벤트 호출
            }
        }
    }
}
