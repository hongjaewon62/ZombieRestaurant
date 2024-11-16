using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [System.Serializable]
    public struct MenuOrder
    {
        public Menu foodMenu;
        public int count;
        public Transform destination;
        public GuestController guest;
        public MenuOrder(Menu menu, int count, Transform destination, GuestController guest)
        {
            this.foodMenu = menu;
            this.count = count;
            this.destination = destination;
            this.guest = guest;
        }
    }

    [System.Serializable]
    public struct OrderDestination
    {
        public Transform destination;

        public OrderDestination(Transform destination)
        {
            this.destination = destination;
        }
    }

    public static OrderManager instance;

    public Menu[] foodMenu;
    public int activeMenuCount = 0;

    private bool guestReceived;

    private Queue<MenuOrder> orderQueue = new Queue<MenuOrder>();
    private Queue<MenuOrder> tempOrderQueue = new Queue<MenuOrder>();
    private Queue<OrderDestination> destinationQueue = new Queue<OrderDestination>();

    private void Awake()
    {
        instance = this;
    }

    public void GuestOrder(int menuIndex, int menuCount, Transform target, GuestController guest)
    {
        int tempIndex = menuIndex;
        MenuOrder newOrder = new MenuOrder(foodMenu[tempIndex], menuCount, target, guest);

        guestReceived = false;
        // 큐에 새로운 주문 추가
        tempOrderQueue.Enqueue(newOrder);
    }

    public void AddOrder(int menuIndex, int menuCount, Transform target, GuestController guest)      // 손님이 주문을 이걸로
    {
        // 새로운 메뉴 주문 생성
        MenuOrder newOrder = new MenuOrder(foodMenu[menuIndex], menuCount, target, guest);
        OrderDestination newDestination = new OrderDestination(target);

        // 큐에 새로운 주문 추가
        orderQueue.Enqueue(newOrder);
        destinationQueue.Enqueue(newDestination);

        Debug.Log($"주문 추가됨: {newOrder.foodMenu.name}, 수량: {newOrder.count}, 주문 위치 : {target.gameObject.name}, 주문 손님 : {guest}");
    }

    public void FoodReceived(bool isReceived)
    {
        guestReceived = isReceived;
    }

    public bool GetFoodReceived()
    {
        return guestReceived;
    }

    public MenuOrder GetNextOrder()
    {
        // 다음 주문을 큐에서 꺼냄
        if (orderQueue.Count > 0)
        {
            return orderQueue.Dequeue();
        }
        else
        {
            Debug.Log("큐에 주문이 없습니다!");
            return new MenuOrder(); // 기본값으로 반환
        }
    }

    public OrderDestination GetNextOrderDestination()
    {
        // 다음 목적지를 큐에서 꺼냄
        if (destinationQueue.Count > 0)
        {
            return destinationQueue.Dequeue();
        }
        else
        {
            Debug.Log("주문이 없습니다!");
            return new OrderDestination(); // 기본값으로 반환
        }
    }

    public Queue<MenuOrder> GetOrderQueue()         // 큐 게터
    {
        return orderQueue; // 큐를 반환
    }

    public Queue<MenuOrder> GetTempOrderQueue()         // 큐 게터
    {
        return tempOrderQueue; // 큐를 반환
    }

    public Queue<OrderDestination> GetOrderDestinationQueue()         // 큐 게터
    {
        return destinationQueue; // 큐를 반환
    }
}
