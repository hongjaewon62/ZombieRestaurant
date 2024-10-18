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

    private Transform destination;
    private TargetOrder targetOrder;

    [SerializeField]
    private GameObject targetParent;            // TargetCounter�� �޾ƿ��� ���� �θ� ������Ʈ
    private TargetCounter[] targetCounter;

    [SerializeField]
    private GameObject progressBar;
    private CircularProgressBar circularProgressBar;

    Quaternion targetRotation = Quaternion.Euler(0, 0, 0);            // �ֹ��� �ٶ� �� ��ġ


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

        foreach (TargetCounter target in targetCounter)
        {
            target.OnReadyChanged += HandleReadyChanged;
        }
    }

    private void Update()
    {
        if (agent.isStopped && gameObject.transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4f);
            Debug.Log("����");
        }
    }

    private void HandleReadyChanged(TargetCounter target)
    {
        Debug.Log("isReady�� true�� TargetCounter �߰�." + target.gameObject.name);
        destination = target.transform.GetChild(0);
        targetOrder = target.transform.GetChild(0).GetComponent<TargetOrder>();

        if (destination != null)
        {
            agent.SetDestination(destination.position);
            Debug.Log("�ֹ�");
        }

        //TakeOrder();                                        // �ֹ�
    }

    private IEnumerator TakeOrder()
    {
        if(targetOrder.isActive)
        {
            agent.isStopped = true;
            anim.SetBool("Idle", true);
            yield return new WaitForSeconds(0.5f);
            progressBar.SetActive(true);
            circularProgressBar.timeMax = orderTime;
        }
        yield return null;
    }

    private void Move()
    {
        agent.isStopped = false;
        anim.SetBool("Idle", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Counter") && !isWorking)
        {
            Debug.Log("�浹");
            targetOrder.isActive = true;
            StartCoroutine(TakeOrder());
        }
    }
}
