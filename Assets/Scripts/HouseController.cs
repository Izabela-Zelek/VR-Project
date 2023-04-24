using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles teleportation of player when entering capsule to enter or leave player house
/// </summary>
public class HouseController : MonoBehaviour
{
    public bool EnterHouse;

    /// <summary>
    /// On collision with trigger object, teleports object to either the player house or player farm
    /// </summary>
    /// <param name="other"></param>
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
