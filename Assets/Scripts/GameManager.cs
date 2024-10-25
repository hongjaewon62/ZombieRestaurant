using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int money = 0;

    [SerializeField]
    private TextMeshProUGUI moneyText;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        moneyText.text = money.ToString();
    }
}
