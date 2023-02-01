using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCapsule : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().EnterMap(true);
        }
    }
}
