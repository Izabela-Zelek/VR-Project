using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI moneyText;

    private int money = 100;
    // Start is called before the first frame update
    void Start()
    {
        moneyText.text = "$" + money.ToString();
    }

    public void UpdateMoney(int newMoney)
    {
        money = newMoney;
        moneyText.text = "$" + money.ToString();
    }

    public int GetMoney()
    {
        return money;
    }
}
