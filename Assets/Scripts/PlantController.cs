using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the storage of all possible plant types to stop constant loading of prefabs
/// </summary>
public class PlantController : MonoBehaviour
{
    public GameObject turnip;
    public GameObject carrot;
    public GameObject tomato;   
    public GameObject FruitTurnip;
    public GameObject FruitCarrot;
    public GameObject FruitTomato;
    /// <summary>
    /// Returns the correct plant gameobject based on the passed in plant name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Returns the correct fruit gameobject based on the passed in fruit name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
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
