using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellItemController : MonoBehaviour
{
    [SerializeField]
    private int Price;
   
    public int GetPrice()
    {
        return Price;
    }
}
