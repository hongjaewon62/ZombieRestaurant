using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuestController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    [SerializeField]
    private float speed = 1f;       // 이동속도

    [SerializeField]
    private GameObject[] destination;           // 목적지 배열
    private int currentDestinationIndex = 0;    // 목적지 인덱스

    [SerializeField]
    private GameObject targetParent;            // TargetCounter을 받아오기 위한 부모 오브젝트
    private TargetCounter[] targetCounter;

    [SerializeField]
    private GameObject orderIcon;

    Quaternion targetRotation = Quaternion.Euler(0, 180, 0);            // 주문할 바라볼 곳 위치

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        agent.speed = speed;
        agent.SetDestination(destination[0].transform.position);
        for(int i = 1; i < destination.Length; i++)
        {
            targetCounter = targetParent.GetComponentsInChildren<TargetCounter>();
        }
    }

    private void Update()
    {
        if(agent.isStopped && gameObject.transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
            Debug.Log("실행");
        }
    }

    private void SetNextDestination()
    {
        agent.SetDestination(destination[currentDestinationIndex].transform.position);      // 목적지 이동
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Counter"))
        {
            Debug.Log("충돌");
            StartCoroutine(MenuOrder());
        }

        if (other.CompareTag("Point"))
        {
            for(int i = 1; i < destination.Length; i++)
            {
                if(targetCounter[i].isActive == false)      // 목적지 아무도 없는 곳으로 이동
                {
                    currentDestinationIndex = i;
                    SetNextDestination();
                    targetCounter[i].isActive = true;
                    return;
                }
            }
            //currentDestinationIndex = Random.Range(1, destination.Length);  // 랜덤이 아니라 빈 곳 순서대로 교체
            //gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);    // 도착하면 주문 전에 앞 바라보도록 회전 시키기
        }
    }

    //private void MenuOrder()
    //{
    //    Quaternion targetRotation = Quaternion.Euler(0, 180, 0);
    //    agent.isStopped = true;
    //    if(agent.isStopped)
    //    {
    //        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
    //        gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
    //    }
    //}

    private IEnumerator MenuOrder()
    {
        agent.isStopped = true;                                             // 움직임 멈추기
        anim.SetBool("Idle", true);
        yield return new WaitForSeconds(1f);                                // 1초 대기
        if (agent.isStopped)                                                // 멈췄다면 주문
        {
            orderIcon.SetActive(true);
            // 주문
        }

        yield return null;
    }
}
