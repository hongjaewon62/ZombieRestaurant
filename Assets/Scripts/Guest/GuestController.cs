using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class GuestController : MonoBehaviour
{
    private GuestController guestController;
    private NavMeshAgent agent;
    private Animator anim;

    [SerializeField]
    private float moveSpeed = 1f;       // 이동속도

    public bool isReady;
    public bool isFoodReceived;
    private bool isMovingToDestination;

    [SerializeField]
    private GameObject[] destination;           // 목적지 배열
    public int currentDestinationIndex = 0;    // 목적지 인덱스

    public int targetCount;

    [SerializeField]
    private GameObject targetParent;            // TargetCounter을 받아오기 위한 부모 오브젝트
    private TargetCounter[] targetCounter;

    [SerializeField]
    private GameObject menuImage;

    private int menuIndex = 0;

    [SerializeField]
    private TextMeshProUGUI menuCountText;
    private int count = 0;

    private GameObject[] appearance;      // 생김새 오브젝트       널 오류로 new로 초기화

    Quaternion targetRotation = Quaternion.Euler(0, 180, 0);            // 주문할 바라볼 곳 위치

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        guestController = GetComponent<GuestController>();
    }

    private void OnEnable()
    {
        Debug.Log("count" + gameObject.transform.childCount);
        targetParent = GameObject.FindWithTag("TargetParent");
        appearance = new GameObject[gameObject.transform.childCount - 2]; // 배열 초기화
        for (int i = 0; i < destination.Length; i++)
        {
            GameObject child = targetParent.transform.GetChild(i).gameObject;

            destination[i] = child;
        }

        for (int i = 0; i < 9; i++)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            appearance[i] = child;
        }

        appearance[Random.Range(0, 8)].gameObject.SetActive(true);
    }

    private void Start()
    {
        agent.speed = moveSpeed;
        agent.SetDestination(destination[0].transform.position);
        for(int i = 1; i < destination.Length; i++)
        {
            targetCounter = targetParent.GetComponentsInChildren<TargetCounter>();
        }
        targetCount = targetCounter.Length - 1;
    }

    private void Update()
    {
        if (agent.isStopped && gameObject.transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            Debug.Log("실행");
        }

        if (isFoodReceived && !isMovingToDestination && !agent.pathPending)
        {
            isMovingToDestination = true;
            GameManager.instance.GiveMoney(100);                  // 재화 추가
            //currentDestinationIndex = targetCounter.Length - 1;
            SetNextDestination();
        }

        if(currentDestinationIndex == targetCount && !agent.pathPending)
        {
            SetNextDestination();
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
                if(targetCounter[i].isActive == false && targetCounter[i].isReady == false && !isFoodReceived)      // 목적지 아무도 없는 곳으로 이동
                {
                    currentDestinationIndex = i;
                    SetNextDestination();
                    targetCounter[i].isActive = true;
                    return;
                }
                else
                {
                    currentDestinationIndex = destination.Length - 1;
                    SetNextDestination();
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Counter"))
        {

        }
    }

    private IEnumerator MenuOrder()
    {
        agent.isStopped = true;                                             // 움직임 멈추기
        anim.SetBool("Idle", true);
        yield return new WaitForSeconds(0.5f);                                // 0.5초 대기
        if (agent.isStopped)                                                // 멈췄다면 주문
        {

            menuIndex = Random.Range(0, OrderManager.instance.activeMenuCount);
            count = Random.Range(1, 2);                                     // 주문 개수(나중에 더 많이 구매하도록 수정)
            OrderManager.instance.GuestOrder(menuIndex, count, destination[currentDestinationIndex].transform, guestController);
            menuImage.SetActive(true);
            menuImage.transform.GetChild(menuIndex).gameObject.SetActive(true);
            menuCountText.text = count.ToString();
            isReady = true;
            // 주문
        }

        yield return null;
    }

    public void GoHome()
    {
        isFoodReceived = true;
        isReady = false;
        menuImage.transform.GetChild(menuIndex).gameObject.SetActive(false);
        menuImage.SetActive(false);
        agent.isStopped = false;
        anim.SetBool("Idle", false);
        currentDestinationIndex = 0;
        SetNextDestination();
    }

    //private void RandomMenu()
    //{
    //    for(int i = 0; i < foodMenu.Length; i++)
    //    {
    //        if(foodMenu[i].isActive)
    //        {

    //        }
    //    }
    //}
}
