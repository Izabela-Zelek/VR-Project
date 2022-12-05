using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Produce")
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().UpdateMoney(GameObject.Find("GameManager").GetComponent<GameManager>().GetMoney() + other.GetComponent<FruitController>().price);
            Destroy(other.gameObject);
        }
    }
}
