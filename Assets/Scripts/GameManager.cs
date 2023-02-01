using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


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
    private bool inMap = false;
    public GameObject player;
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

    public void EnterMap(bool enter)
    {
        inMap = enter;

        if(inMap)
        {
            player.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false;
            player.GetComponent<ActionBasedContinuousTurnProvider>().enabled = false;
            player.GetComponent<TeleportationProvider>().enabled = false;
            player.GetComponent<ActivateTeleportationRay>().enabled = false;
            player.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
            player.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
        }
        else
        {
            player.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;
            player.GetComponent<ActionBasedContinuousTurnProvider>().enabled = true;
            player.GetComponent<TeleportationProvider>().enabled = true;
            player.GetComponent<ActivateTeleportationRay>().enabled = true;
            player.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
            player.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
        }
    }
}
