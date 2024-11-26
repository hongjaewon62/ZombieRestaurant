using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject guest;

    public int maxGuest = 1;
    public int currentGuest = 0;

    [SerializeField]
    private float maxCooldown = 7f;
    private float minCooldown = 3f;

    private void Start()
    {
        StartCoroutine(SpawnGuests());
    }

    private void Update()
    {
        //StartCoroutine(SpawnGuests());
    }

    private void SpawnObject()
    {
        Instantiate(guest, gameObject.transform.position, Quaternion.identity);
        currentGuest++;
    }

    private IEnumerator SpawnGuests()
    {
        while(true)
        {
            if(currentGuest < maxGuest)
            {
                SpawnObject();
                yield return new WaitForSeconds(Random.Range(minCooldown, maxCooldown)); // ÄðÅ¸ÀÓ ´ë±â
            }
            else
            {
                yield return null;
            }
        }
    }
}
