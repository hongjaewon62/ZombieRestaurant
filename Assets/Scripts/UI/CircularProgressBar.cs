using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CircularProgressBar : MonoBehaviour
{
    public float time = 0f;
    public float timeMax = 10f;

    [SerializeField]
    private Image fill;

    private void OnDisable()
    {
        time = 0f;
        fill.fillAmount = time;
    }

    private void Update()
    {
        Timer();
    }

    private void Timer()
    {
        if (time < timeMax)
        {
            time += Time.deltaTime;                 // time ����
            fill.fillAmount = time / timeMax;       // fillAmount ������Ʈ
        }

        if (time >= timeMax)
        {
            time = timeMax;
            gameObject.SetActive(false);
        }
    }
}
