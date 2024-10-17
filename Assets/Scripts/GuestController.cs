using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuestController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    [SerializeField]
    private float speed = 1f;       // �̵��ӵ�

    [SerializeField]
    private GameObject[] destination;           // ������ �迭
    private int currentDestinationIndex = 0;    // ������ �ε���

    [SerializeField]
    private GameObject targetParent;            // TargetCounter�� �޾ƿ��� ���� �θ� ������Ʈ
    private TargetCounter[] targetCounter;

    [SerializeField]
    private GameObject orderIcon;

    Quaternion targetRotation = Quaternion.Euler(0, 180, 0);            // �ֹ��� �ٶ� �� ��ġ

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
            Debug.Log("����");
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
                if(targetCounter[i].isActive == false)      // ������ �ƹ��� ���� ������ �̵�
                {
                    currentDestinationIndex = i;
                    SetNextDestination();
                    targetCounter[i].isActive = true;
                    return;
                }
            }
            //currentDestinationIndex = Random.Range(1, destination.Length);  // ������ �ƴ϶� �� �� ������� ��ü
            //gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);    // �����ϸ� �ֹ� ���� �� �ٶ󺸵��� ȸ�� ��Ű��
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
        agent.isStopped = true;                                             // ������ ���߱�
        anim.SetBool("Idle", true);
        yield return new WaitForSeconds(1f);                                // 1�� ���
        if (agent.isStopped)                                                // ����ٸ� �ֹ�
        {
            orderIcon.SetActive(true);
            // �ֹ�
        }

        yield return null;
    }
}
