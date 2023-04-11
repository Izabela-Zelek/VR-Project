using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHouseController : MonoBehaviour
{
    private int _assignedNPC;
    private GameObject _npcObject;
    private int _startMoveTime;
    private bool doOnce = false;
    private bool shopKeeper = false;
    // Start is called before the first frame update
    void Start()
    {
        string[] nameSplit = name.Split(new string[] { "ShopGuyHouse" }, System.StringSplitOptions.None);
        _assignedNPC = int.Parse(nameSplit[1]);
        _npcObject = GameObject.Find("Dude" + _assignedNPC.ToString());
       
    }

    // Update is called once per frame
    void Update()
    {
        if(!doOnce)
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
        if(GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.Hour == _startMoveTime - 1 && GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.Minute == 55)
        {
            if(shopKeeper)
            {
                if(_npcObject.GetComponent<ShopKeeperMover>().WorkToday())
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
