using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCounter : MonoBehaviour
{
    public bool isActive;
    public bool isReady;
    public bool isFood;

    public event Action<TargetCounter> OnReadyChanged; // �̺�Ʈ ����

    private GuestController guest;


    private void Update()
    {
        if(transform.childCount == 0)
        {
            return;
        }
        // trigger ���� ������ ������ ������ �ֹ� ������� �ʴ� ���� �ذ�
        if (isReady)
        {
            transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isActive = true;

        if(other.CompareTag("Guest"))
        {
            guest = other.GetComponent<GuestController>();

            if (guest.isFoodReceived)
            {
                guest.currentDestinationIndex = guest.targetCount;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isActive = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Guest") && isActive)
        {
            isActive = true;
            guest = other.GetComponent<GuestController>();
            if (!isReady && guest.isReady && !guest.isFoodReceived)
            {
                isReady = true;
                OnReadyChanged?.Invoke(this);       // �̺�Ʈ ȣ��
            }
        }

        if(other.CompareTag("Guest") && guest.isFoodReceived && gameObject.tag == "Counter")
        {
            isActive = false;
            isReady = false;
            isFood = false;
            guest.GoHome();
        }
    }
}
