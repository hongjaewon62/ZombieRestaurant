using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    [SerializeField]
    private GuestSpawner spawner;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Guest"))
        {
            Destroy(other.gameObject);
            spawner.currentGuest--;
        }
    }
}
