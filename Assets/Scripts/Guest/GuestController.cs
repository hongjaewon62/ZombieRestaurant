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
    private float moveSpeed = 1f;       // �̵��ӵ�

    public bool isReady;
    public bool isFoodReceived;
    private bool isMovingToDestination;

    [SerializeField]
    private GameObject[] destination;           // ������ �迭
    public int currentDestinationIndex = 0;    // ������ �ε���

    public int targetCount;

    [SerializeField]
    private GameObject targetParent;            // TargetCounter�� �޾ƿ��� ���� �θ� ������Ʈ
    private TargetCounter[] targetCounter;

    [SerializeField]
    private GameObject menuImage;

    [SerializeField]
    private TextMeshProUGUI menuCountText;
    private int count = 0;

    Quaternion targetRotation = Quaternion.Euler(0, 180, 0);            // �ֹ��� �ٶ� �� ��ġ

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        guestController = GetComponent<GuestController>();
    }

    private void OnEnable()
    {
        targetParent = GameObject.FindWithTag("TargetParent");
        for (int i = 0; i < destination.Length; i++)
        {
            GameObject child = targetParent.transform.GetChild(i).gameObject;

            destination[i] = child;
        }
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
        if(agent.isStopped && gameObject.transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            Debug.Log("����");
        }

        if (isFoodReceived && !isMovingToDestination && !agent.pathPending)
        {
            isMovingToDestination = true;
            GameManager.instance.money += 100;                  // ��ȭ �߰�
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
        agent.SetDestination(destination[currentDestinationIndex].transform.position);      // ������ �̵�
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Counter"))
        {
            Debug.Log("�浹");
            StartCoroutine(MenuOrder());
        }

        if (other.CompareTag("Point"))
        {
            for(int i = 1; i < destination.Length; i++)
            {
                if(targetCounter[i].isActive == false && targetCounter[i].isReady == false && !isFoodReceived)      // ������ �ƹ��� ���� ������ �̵�
                {
                    currentDestinationIndex = i;
                    SetNextDestination();
                    targetCounter[i].isActive = true;
                    return;
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
        agent.isStopped = true;                                             // ������ ���߱�
        anim.SetBool("Idle", true);
        yield return new WaitForSeconds(0.5f);                                // 0.5�� ���
        if (agent.isStopped)                                                // ����ٸ� �ֹ�
        {

            int menuIndex = Random.Range(0, OrderManager.instance.activeMenuCount);
            count = Random.Range(1, 2);                                     // �ֹ� ����(���߿� �� ���� �����ϵ��� ����)
            OrderManager.instance.GuestOrder(menuIndex, count, destination[currentDestinationIndex].transform, guestController);
            menuImage.SetActive(true);
            menuCountText.text = count.ToString();
            isReady = true;
            // �ֹ�
        }

        yield return null;
    }

    public void GoHome()
    {
        isFoodReceived = true;
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
