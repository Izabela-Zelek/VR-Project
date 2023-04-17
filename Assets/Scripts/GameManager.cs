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
    private int _birdDirection;
    private int _randTime;
    private GameObject _bird;
    // Start is called before the first frame update
    void Start()
    {
        moneyText.text = "$" + money.ToString();
        _bird = Resources.Load("BirdForm") as GameObject;
        _birdDirection = Random.Range(0, 4);
        _randTime = Random.Range(2, 4);
        StartCoroutine(birdTimer());
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

    public void MinusMoney(int newMoney)
    {
        money = newMoney;
        moneyText.text = "$" + money.ToString();
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

    public bool IsInMap()
    {
        return inMap;
    }

    private void SpawnBird()
    {
        Vector3 spawnPos = Vector3.zero;
        Vector3 childOne = transform.GetChild(0).position;
        Vector3 childTwo = transform.GetChild(1).position;
        Vector3 headingPos = Vector3.zero;
        float randX;
        float randZ;
        switch (_birdDirection)
        {
            case 0:
                randX = Random.Range(childTwo.x, childOne.x);
                spawnPos = new Vector3(randX, childOne.y, childOne.z);
                headingPos = new Vector3(spawnPos.x, spawnPos.y, childTwo.z);
                break;
            case 1:
                randX = Random.Range(childTwo.x, childOne.x);
                spawnPos = new Vector3(randX, childTwo.y, childTwo.z);
                headingPos = new Vector3(spawnPos.x, spawnPos.y, childOne.z);
                break;
            case 2:
                randZ = Random.Range(childTwo.z, childOne.z);
                spawnPos = new Vector3(childOne.x, childOne.y, randZ);
                headingPos = new Vector3(childTwo.x, spawnPos.y, spawnPos.z);
                break;
            case 3:
                randZ = Random.Range(childTwo.z, childOne.z);
                spawnPos = new Vector3(childTwo.x, childTwo.y, randZ);
                headingPos = new Vector3(childOne.x, spawnPos.y, spawnPos.z);
                break;
        }
        GameObject newBird = Instantiate(_bird, spawnPos, Quaternion.identity, transform);
        newBird.GetComponent<BirdFormController>().setDirection(headingPos);
    }

    IEnumerator birdTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(_randTime);
            SpawnBird();
            _birdDirection = Random.Range(0, 4);
            _randTime = Random.Range(20, 40);
        }
    }
}
