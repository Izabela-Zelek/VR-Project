using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles entering the map/level editor
/// </summary>
public class MapCapsule : MonoBehaviour
{
    /// <summary>
    /// Upon colliding with player, calls for the EnterMap function to limit movability of the player while editing environment
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().EnterMap(true);
        }
    }
}
