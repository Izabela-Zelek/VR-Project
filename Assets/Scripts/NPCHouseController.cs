using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the pairing between the NPCs and their houses
/// </summary>
public class NPCHouseController : MonoBehaviour
{
    private int _assignedNPC;
    private GameObject _npcObject;
    private int _startMoveTime;
    private bool doOnce = false;
    private bool shopKeeper = false;

    /// <summary>
    /// Separates the id of the house from the name and finds the NPC with the corresponding name
    /// </summary>
    void Start()
    {
        string[] nameSplit = name.Split(new string[] { "ShopGuyHouse" }, System.StringSplitOptions.None);
        if (int.TryParse(nameSplit[1], out _assignedNPC))
        {
            _npcObject = GameObject.Find("Dude" + _assignedNPC.ToString());
        }
        else
        {
            _assignedNPC = -1;
        }
    }

    /// <summary>
    /// Grabs the wake up time of the assigned NPC and enables them when the time of the day is equal
    /// </summary>
    void Update()
    {
        if (_npcObject)
        {
            if (!doOnce)
            {
                if (_npcObject.GetComponent<ShopKeeperMover>())
                {
                    _startMoveTime = _npcObject.GetComponent<ShopKeeperMover>().GetStartTime();
                    shopKeeper = true;
                }
                else if (_npcObject.GetComponent<PathMover>().isActiveAndEnabled)
                {
                    _startMoveTime = _npcObject.GetComponent<PathMover>().GetStartTime();
                }
                else
                {
                    _startMoveTime = _npcObject.GetComponent<NPCContoller>().GetStartTime();
                }
                _npcObject.SetActive(false);
                doOnce = true;
            }
            if (GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.Hour == _startMoveTime - 1 && GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.Minute == 55)
            {
                if (shopKeeper)
                {
                    if (_npcObject.GetComponent<ShopKeeperMover>().WorkToday())
                    {
                        _npcObject.SetActive(true);

                    }
                }
                else
                {
                    _npcObject.SetActive(true);
                }
            }
        }
    }
}
