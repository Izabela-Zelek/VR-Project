using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    public GameObject turnip;
    public GameObject carrot;
    public GameObject tomato;

    public GameObject getPlant(string name)
    {
        GameObject plant = new GameObject();
        switch (name)
        {
            case "Turnip":
                plant = turnip.gameObject;
                break;
            case "Carrot":
                plant = carrot.gameObject;
                break;
            case "Tomato":
                plant = tomato.gameObject;
                break;
            default:
                break;
        }
        return plant;
    }
}
