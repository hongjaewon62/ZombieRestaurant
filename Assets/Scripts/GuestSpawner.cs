using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject guest;

    public int maxGuest = 1;
    public int currentGuest = 0;

    private float cooldown = 3f;
    
    private void Update()
    {
            StartCoroutine(SpawnGuests());
    }

    private void SpawnObject()
    {
        Instantiate(guest, gameObject.transform.position, Quaternion.identity);
        currentGuest++;
    }

    private IEnumerator SpawnGuests()
    {
        while (currentGuest < maxGuest)
        {
            SpawnObject();
            yield return new WaitForSeconds(cooldown); // ÄðÅ¸ÀÓ ´ë±â
        }
    }
}
