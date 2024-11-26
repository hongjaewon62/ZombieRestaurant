using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    private float moveSpeed = 2f;
    private float orderTime = 2f;

    private bool isWorking;
    private bool isOrdering;
    private bool isCooking;
    public bool isFood;

    private Transform tempDestination;

    private Transform destination;
    private TargetOrder targetOrder;

    [SerializeField]
    private GameObject targetParent;            // TargetCounter�� �޾ƿ��� ���� �θ� ������Ʈ
    private TargetCounter[] targetCounter;

    [SerializeField]
    private GameObject cookingTargetParent;
    private TargetCooking[] targetCooking;
    private TargetCooking tempTargetCooking;

    private Queue<OrderManager.OrderDestination> destinations;
    private Queue<OrderManager.MenuOrder> guestOrders;
    private Queue<OrderManager.MenuOrder> orders;

    [SerializeField]
    private GameObject progressBar;
    private CircularProgressBar circularProgressBar;

    //Quaternion targetRotation = Quaternion.Euler(0, 0, 0);            // �ֹ��� �ٶ� �� ��ġ


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        circularProgressBar = progressBar.GetComponent<CircularProgressBar>();
        agent.speed = moveSpeed;

        for (int i = 1; i < targetParent.transform.childCount; i++)
        {
            targetCounter = targetParent.GetComponentsInChildren<TargetCounter>();
        }

        for(int i = 0; i < cookingTargetParent.transform.childCount; i++)
        {
            // ��Ȱ��ȭ�� target�� �ȵ�   Ȱ��ȭ�� �� ���� �߰��� ��
            targetCooking = cookingTargetParent.GetComponentsInChildren<TargetCooking>();
        }

        foreach (TargetCounter target in targetCounter)
        {
            target.OnReadyChanged += HandleReadyChanged;
        }
    }

    private void Update()
    {
        //if (agent.isStopped && gameObject.transform.rotation != targetRotation)
        //{
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4f);         // ������ ��, �ֹ����� �� �ٸ��� �� ��   �ٶ󺸴� ������ �� ������Ʈ�� ���ϰ�
        //    Debug.Log("����");
        //}

        if(guestOrders == null || orders == null)         // �ֹ��� ���ٸ� ����
        {
            return;
        }

        if(guestOrders.Count == 0 && orders.Count > 0)    // ����
        {
            MoveToCookingStation();
        }
    }

    private void HandleReadyChanged(TargetCounter target)
    {
        destinations = OrderManager.instance.GetOrderDestinationQueue();
        guestOrders = OrderManager.instance.GetTempOrderQueue();
        Debug.Log("isReady�� true�� TargetCounter �߰�." + target.gameObject.name);
        if(guestOrders.Count > 0)
        {
            destination = guestOrders.Peek().destination.GetChild(0).transform;
        }
        if(target.transform.GetChild(0) != null)
        {
            targetOrder = target.transform.GetChild(0).GetComponent<TargetOrder>();
        }

        if (destination != null)
        {
            //tempDestination = destination;
            agent.SetDestination(destination.position);
            Debug.Log("�ֹ�");
        }

        //TakeOrder();                                        // �ֹ�
    }

    private IEnumerator TakeOrder()         // �ֹ� �ڷ�ƾ
    {
        if(targetOrder.isActive)
        {
            Stop();
            yield return new WaitForSeconds(0.5f);
            ProgressBar(orderTime);
            yield return new WaitForSeconds(orderTime);
            AddGuestOrder();
            OrderManager.instance.GetNextOrderDestination();
            Debug.Log("�ֹ� �� : " +  destinations.Count);
            MoveDestination();
        }
        yield return null;
    }

    private void MoveDestination()
    {
        Move();
        if(guestOrders.Count > 0 && !isCooking)
        {

            destination = guestOrders.Peek().destination.GetChild(0).transform;
            if (destination != null)
            {
                //tempDestination = destination;
                Debug.Log(guestOrders.Peek());
                agent.SetDestination(destination.position);
                Debug.Log("�ֹ�");
            }
        }
    }

    private void AddGuestOrder()                    // �մ� �ֹ� ť
    {
        OrderManager.MenuOrder firstOrder = guestOrders.Dequeue();
        int menuIndex = Array.IndexOf(OrderManager.instance.foodMenu, firstOrder.foodMenu);

        Debug.Log(menuIndex +", " + firstOrder.count + ", " + firstOrder.destination);

        OrderManager.instance.AddOrder(menuIndex, firstOrder.count, firstOrder.destination, firstOrder.guest);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Counter") && !isCooking)
        {
            Debug.Log("�浹");
            targetOrder.isActive = true;
            StartCoroutine(TakeOrder());
            CheckOrderQueue();
            // �����Ϸ� �� �� �ֹ������� ���°� ������ ��
        }

        if(other.CompareTag("Cooking") && isCooking)
        {
            tempTargetCooking = other.gameObject.GetComponent<TargetCooking>();
            StartCoroutine(Cooking());
        }

        if(other.CompareTag("Counter") && isFood)
        {
            targetOrder.isFood = true;
        }
    }

    private void CheckOrderQueue()                      // �ֹ� üũ
    {
        orders = OrderManager.instance.GetOrderQueue();
        Debug.Log($"���� ť�� �ֹ� ��: {orders.Count}");

        foreach (var order in orders)
        {
            Debug.Log($"�ֹ�: {order.foodMenu.name}, ����: {order.count}");
        }
    }

    private void MoveToCookingStation()             // �����ϴ� ������ �̵� �Լ�
    {
        if(isCooking)
        {
            return;
        }

        isCooking = true;

        orders = OrderManager.instance.GetOrderQueue();

        for(int i = 0; i < targetCooking.Length; i++)
        {
            if(targetCooking[i].isActive == false && targetCooking[i].menu == orders.Peek().foodMenu)
            {
                tempDestination = orders.Peek().destination;
                agent.SetDestination(targetCooking[i].transform.position);
            }
        }
    }

    private IEnumerator Cooking()                           // ���� �ڷ�ƾ
    {
        Stop();
        yield return new WaitForSeconds(0.3f);
        ProgressBar(tempTargetCooking.cookingTime);
        yield return new WaitForSeconds(tempTargetCooking.cookingTime);
        Move();
        agent.SetDestination(tempDestination.GetChild(0).position);         // �ֹ� ���� ������ �̵�
        isFood = true;
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            GiveFood();
        }

        isCooking = false;
    }

    private void GiveFood()
    {
        OrderManager.instance.FoodReceived(true);
        isCooking = false;
        isFood = false;
        OrderManager.instance.GetNextOrder();
    }

    private void ProgressBar(float time)            // ���� ��
    {
        progressBar.SetActive(true);
        circularProgressBar.timeMax = time;
    }

    private void Move()                         // �̵�
    {
        agent.isStopped = false;
        anim.SetBool("Idle", false);
    }

    private void Stop()                         // ����
    {
        agent.isStopped = true;
        anim.SetBool("Idle", true);
    }
}
