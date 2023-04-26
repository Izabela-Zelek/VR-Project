using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles player money, entering and leaving the map editor, and bird flock spawning
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _moneyText;
    [SerializeField]
    private CapsuleController _sleepArea;

    public bool Asleep = false;
    public bool ShopOpen = true;
    public GameObject Player;

    private int _money = 100;
    private int _addedMoney = 0;
    private bool _addMoney = false;
    private bool _inMap = false;
    private int _birdDirection;
    private int _randTime;
    private GameObject _bird;
    
    /// <summary>
    /// Sets initial money amount and watch text
    /// Loads bird flock prefab
    /// Randomises flock direction and spawn time
    /// </summary>
    private void Start()
    {
        _moneyText.text = "$" + _money.ToString();
        _bird = Resources.Load("BirdForm") as GameObject;
        _birdDirection = Random.Range(0, 4);
        _randTime = Random.Range(2, 4);
        StartCoroutine(birdTimer());
    }

    /// <summary>
    /// Updates player money on player watch if player is asleep
    /// </summary>
    private void Update()
    {
        if (_addMoney && _sleepArea.asleep)
        {
            _money = _money + _addedMoney;
            _moneyText.text = "$" + _money.ToString();
            _addedMoney = 0;
            _addMoney = false;
        }
    }

    /// <summary>
    /// Upon sale of item, increments new money amount by given value
    /// </summary>
    /// <param name="newMoney"></param>
    public void UpdateMoney(int newMoney)
    {
        _addedMoney = _addedMoney + newMoney;
        _addMoney = true;
    }

    /// <summary>
    /// Immediately updates money when player purchases items
    /// </summary>
    /// <param name="newMoney"></param>
    public void MinusMoney(int newMoney)
    {
        _money = newMoney;
        _moneyText.text = "$" + _money.ToString();
    }

    /// <summary>
    /// Returns current money amount
    /// </summary>
    /// <returns></returns>
    public int GetMoney()
    {
        return _money;
    }

    /// <summary>
    /// Disables player movement and rotation upon entering level editor, disables teleportation ray, enables level editor ray
    /// Enables player movement and rotation upon laeving level editor, enables teleportation ray, disables level editor ray
    /// </summary>
    /// <param name="enter"></param>
    public void EnterMap(bool enter)
    {
        _inMap = enter;

        if(_inMap)
        {
            Player.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false;
            Player.GetComponent<ActionBasedContinuousTurnProvider>().enabled = false;
            Player.GetComponent<TeleportationProvider>().enabled = false;
            Player.GetComponent<ActivateTeleportationRay>().enabled = false;
            Player.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
            Player.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
        }
        else
        {
            Player.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;
            Player.GetComponent<ActionBasedContinuousTurnProvider>().enabled = true;
            Player.GetComponent<TeleportationProvider>().enabled = true;
            Player.GetComponent<ActivateTeleportationRay>().enabled = true;
            Player.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
            Player.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Returns whether or not player is currently editing level
    /// </summary>
    /// <returns></returns>
    public bool IsInMap()
    {
        return _inMap;
    }

    /// <summary>
    /// Sets spawn position of bird flock depending of randomised flock direction
    /// Spawns flock
    /// </summary>
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

    /// <summary>
    /// Waits randomised time to spawn bird flock
    /// Calls SpawnBird function
    /// Randomises direction and spawn time of future flock
    /// </summary>
    /// <returns></returns>
    private IEnumerator birdTimer()
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
