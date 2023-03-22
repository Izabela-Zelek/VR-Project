using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseController : MonoBehaviour
{
    public bool EnterHouse;

    private void OnTriggerEnter(Collider other)
    {
        if(EnterHouse)
        {
            other.transform.position = GameObject.Find("LeaveHouse").transform.GetChild(0).transform.position;
        }
        else
        {
            other.transform.position = GameObject.Find("EnterHouse").transform.GetChild(0).transform.position;
        }
    }
}
