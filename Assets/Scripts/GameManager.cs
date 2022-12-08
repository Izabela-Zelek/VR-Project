using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI moneyText;
    [SerializeField]
    private CapsuleController sleepArea;
    private int money = 100;
    private int addedMoney = 0;
    private bool addMoney = false;
    public bool asleep = false;
    public bool shopOpen = true;
    // Start is called before the first frame update
    void Start()
    {
        moneyText.text = "$" + money.ToString();
    }

    private void Update()
    {
        
        if (addMoney && sleepArea.asleep)
        {
            money = money + addedMoney;
            moneyText.text = "$" + money.ToString();
            addedMoney = 0;
            addMoney = false;
        }    
    }

    public void UpdateMoney(int newMoney)
    {
        addedMoney = addedMoney + newMoney;
        addMoney = true;
    }

    public int GetMoney()
    {
        return money;
    }
}
