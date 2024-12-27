using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int money = 0;
    public int moneyPower = 1;

    [SerializeField]
    private TextMeshProUGUI moneyText;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        moneyText.text = money.ToString();

        //if (Input.GetKey(KeyCode.Q))
        //{
        //    Time.timeScale = 7.5f;  // 2¹è¼Ó
        //}
        //else if (Input.GetKey(KeyCode.W))
        //{
        //    Time.timeScale = 1f;  
        //}
    }

    public void GiveMoney(int money)
    {
        this.money += money * moneyPower;
    }
}
