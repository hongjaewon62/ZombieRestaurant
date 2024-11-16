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
        // ť�� ���ο� �ֹ� �߰�
        tempOrderQueue.Enqueue(newOrder);
    }

    public void AddOrder(int menuIndex, int menuCount, Transform target, GuestController guest)      // �մ��� �ֹ��� �̰ɷ�
    {
        // ���ο� �޴� �ֹ� ����
        MenuOrder newOrder = new MenuOrder(foodMenu[menuIndex], menuCount, target, guest);
        OrderDestination newDestination = new OrderDestination(target);

        // ť�� ���ο� �ֹ� �߰�
        orderQueue.Enqueue(newOrder);
        destinationQueue.Enqueue(newDestination);

        Debug.Log($"�ֹ� �߰���: {newOrder.foodMenu.name}, ����: {newOrder.count}, �ֹ� ��ġ : {target.gameObject.name}, �ֹ� �մ� : {guest}");
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
        // ���� �ֹ��� ť���� ����
        if (orderQueue.Count > 0)
        {
            return orderQueue.Dequeue();
        }
        else
        {
            Debug.Log("ť�� �ֹ��� �����ϴ�!");
            return new MenuOrder(); // �⺻������ ��ȯ
        }
    }

    public OrderDestination GetNextOrderDestination()
    {
        // ���� �������� ť���� ����
        if (destinationQueue.Count > 0)
        {
            return destinationQueue.Dequeue();
        }
        else
        {
            Debug.Log("�ֹ��� �����ϴ�!");
            return new OrderDestination(); // �⺻������ ��ȯ
        }
    }

    public Queue<MenuOrder> GetOrderQueue()         // ť ����
    {
        return orderQueue; // ť�� ��ȯ
    }

    public Queue<MenuOrder> GetTempOrderQueue()         // ť ����
    {
        return tempOrderQueue; // ť�� ��ȯ
    }

    public Queue<OrderDestination> GetOrderDestinationQueue()         // ť ����
    {
        return destinationQueue; // ť�� ��ȯ
    }
}
