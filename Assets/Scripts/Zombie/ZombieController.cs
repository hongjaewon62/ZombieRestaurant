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
    private GameObject targetParent;            // TargetCounter을 받아오기 위한 부모 오브젝트
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

    //Quaternion targetRotation = Quaternion.Euler(0, 0, 0);            // 주문할 바라볼 곳 위치


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
            // 비활성화인 target은 안들어감   활성화할 때 같이 추가할 것
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
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4f);         // 조리할 때, 주문받을 때 다르게 할 것   바라보는 방향을 그 오브젝트를 향하게
        //    Debug.Log("실행");
        //}

        if(guestOrders == null || orders == null)         // 주문이 없다면 리턴
        {
            return;
        }

        if(guestOrders.Count == 0 && orders.Count > 0)    // 조리
        {
            MoveToCookingStation();
        }
    }

    private void HandleReadyChanged(TargetCounter target)
    {
        destinations = OrderManager.instance.GetOrderDestinationQueue();
        guestOrders = OrderManager.instance.GetTempOrderQueue();
        Debug.Log("isReady가 true인 TargetCounter 발견." + target.gameObject.name);
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
            Debug.Log("주문");
        }

        //TakeOrder();                                        // 주문
    }

    private IEnumerator TakeOrder()         // 주문 코루틴
    {
        if(targetOrder.isActive)
        {
            Stop();
            yield return new WaitForSeconds(0.5f);
            ProgressBar(orderTime);
            yield return new WaitForSeconds(orderTime);
            AddGuestOrder();
            OrderManager.instance.GetNextOrderDestination();
            Debug.Log("주문 수 : " +  destinations.Count);
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
                Debug.Log("주문");
            }
        }
    }

    private void AddGuestOrder()                    // 손님 주문 큐
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
            Debug.Log("충돌");
            targetOrder.isActive = true;
            StartCoroutine(TakeOrder());
            CheckOrderQueue();
            // 조리하러 갈 때 주문받으러 가는것 수정할 것
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

    private void CheckOrderQueue()                      // 주문 체크
    {
        orders = OrderManager.instance.GetOrderQueue();
        Debug.Log($"현재 큐의 주문 수: {orders.Count}");

        foreach (var order in orders)
        {
            Debug.Log($"주문: {order.foodMenu.name}, 수량: {order.count}");
        }
    }

    private void MoveToCookingStation()             // 조리하는 곳으로 이동 함수
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

    private IEnumerator Cooking()                           // 조리 코루틴
    {
        Stop();
        yield return new WaitForSeconds(0.3f);
        ProgressBar(tempTargetCooking.cookingTime);
        yield return new WaitForSeconds(tempTargetCooking.cookingTime);
        Move();
        agent.SetDestination(tempDestination.GetChild(0).position);         // 주문 받은 곳으로 이동
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

    private void ProgressBar(float time)            // 진행 바
    {
        progressBar.SetActive(true);
        circularProgressBar.timeMax = time;
    }

    private void Move()                         // 이동
    {
        agent.isStopped = false;
        anim.SetBool("Idle", false);
    }

    private void Stop()                         // 멈춤
    {
        agent.isStopped = true;
        anim.SetBool("Idle", true);
    }
}
