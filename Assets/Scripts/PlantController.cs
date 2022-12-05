using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    public GameObject turnip;
    public GameObject carrot;
    public GameObject tomato;   
    public GameObject FruitTurnip;
    public GameObject FruitCarrot;
    public GameObject FruitTomato;

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

    public GameObject getFruit(string name)
    {
        GameObject plant = new GameObject();
        switch (name)
        {
            case "Turnip":
                plant = FruitTurnip.gameObject;
                break;
            case "Carrot":
                plant = FruitCarrot.gameObject;
                break;
            case "Tomato":
                plant = FruitTomato.gameObject;
                break;
            default:
                break;
        }
        return plant;
    }
}
