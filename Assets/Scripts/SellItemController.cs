using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles storing the price of the item when sold
/// </summary>
public class SellItemController : MonoBehaviour
{
    [SerializeField]
    private int Price;
   
    public int GetPrice()
    {
        return Price;
    }
}
