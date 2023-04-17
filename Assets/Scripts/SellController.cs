using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Produce")
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().UpdateMoney(other.GetComponent<FruitController>().price);
            Destroy(other.gameObject);
        }
        else if(other.tag == "ToSell")
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().UpdateMoney(other.GetComponent<SellItemController>().GetPrice());
            Destroy(other.gameObject);
        }
    }
}
