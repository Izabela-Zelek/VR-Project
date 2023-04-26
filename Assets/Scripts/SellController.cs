using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the selling of produce and picked up items
/// </summary>
public class SellController : MonoBehaviour
{
    /// <summary>
    /// Upon colliding with a harvested item or a picked up item(eg. log), grabs the price of the item and adds to the player money. Then Destroys the item.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Produce")
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().UpdateMoney(other.GetComponent<FruitController>().Price);
            Destroy(other.gameObject);
        }
        else if(other.tag == "ToSell")
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().UpdateMoney(other.GetComponent<SellItemController>().GetPrice());
            Destroy(other.gameObject);
        }
    }
}
