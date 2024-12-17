using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;

public class UpgradeAbility : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI moneyText;
    [SerializeField]
    private TextMeshProUGUI moneyLevelText;

    [SerializeField]
    private Button moneyButton;

    // 기본 가격
    private int moneyBasePrice = 1000;

    private float growthRate = 1.4f;

    // 현재 레벨
    private int moneyCurrentLevel = 1;

    private int moneyPrice = 0;

    [SerializeField]
    private ZombieController zombie;

    [SerializeField]
    private TextMeshProUGUI speedText;

    [SerializeField]
    private Button speedButton;

    private int speedBasePrice = 500;

    private int speedCurrentLevel = 1;

    private int speedPrice = 0;

    private void Start()
    {
        moneyLevelText.text = "수익 X " + (moneyCurrentLevel + 1).ToString();
    }

    private void Update()
    {
        MoneyTextChange();
        SpeedTextChange();
    }

    private void  MoneyTextChange()
    {
        if (moneyCurrentLevel > 1)
        {
            moneyPrice = MoneyUpgradePrice(moneyCurrentLevel);
        }
        else
        {
            moneyPrice = moneyBasePrice;
        }

        if (moneyCurrentLevel >= 20)
        {
            moneyText.text = "MAX LEVEL";
        }
        else
        {
            moneyText.text = moneyPrice.ToString();
        }

        if (GameManager.instance.money >= moneyPrice && moneyCurrentLevel < 20)
        {
            moneyButton.interactable = true;
        }
        else
        {
            moneyButton.interactable = false;
        }
    }

    // 업그레이드 가격 계산
    private int MoneyUpgradePrice(int level)
    {
        return Mathf.CeilToInt(moneyBasePrice * Mathf.Pow(growthRate, level));
    }

    public void MoneyLevelUp()
    {
        if(GameManager.instance.money >= moneyPrice)
        {
            moneyCurrentLevel++;

            moneyLevelText.text = "수익 X " + (moneyCurrentLevel + 1).ToString();
            GameManager.instance.moneyPower = moneyCurrentLevel;
            GameManager.instance.money -= moneyPrice;
        }
    }

    private int SpeedUpgradePrice(int level)
    {
        return Mathf.CeilToInt(speedBasePrice * Mathf.Pow(growthRate, level));
    }

    public void SpeedLevelUp()
    {
        if (GameManager.instance.money >= speedPrice)
        {
            speedCurrentLevel++;

            zombie.GetComponent<NavMeshAgent>().speed += 0.2f;
            GameManager.instance.money -= speedPrice;
        }
    }

    private void SpeedTextChange()
    {
        if (speedCurrentLevel > 1)
        {
            speedPrice = SpeedUpgradePrice(speedCurrentLevel);
        }
        else
        {
            speedPrice = speedBasePrice;
        }

        if (speedCurrentLevel >= 15)
        {
            speedText.text = "MAX LEVEL";
        }
        else
        {
            speedText.text = speedPrice.ToString();
        }

        if (GameManager.instance.money >= speedPrice && speedCurrentLevel < 15)
        {
            speedButton.interactable = true;
        }
        else
        {
            speedButton.interactable = false;
        }
    }
}
