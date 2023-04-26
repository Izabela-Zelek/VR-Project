using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines all 5 possible states of planting fields
/// </summary>
public enum PlantState
{
    Bare = 0,
    Seed = 1,
    Growing = 2,
    Grown = 3,
    Fruit = 4
}
/// <summary>
/// Handles the change of states in planting fields
/// </summary>
public class FarmScript : MonoBehaviour
{
    public GameObject seeds;
    public GameObject growingPlant;
    private GameObject grownPlant;
    private GameObject fruitType;
    public Material dryMat;
    public Material wetMat;
    public PlantState plantState;
    private int currentDay;
    private bool watered = false;
    private CapsuleController sleepArea;

    private ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];

    /// <summary>
    /// Sets initial state to Base - no seeds planted
    /// Saves day of creation
    /// </summary>
    private void Start()
    {
        plantState = PlantState.Bare;
        currentDay = GameObject.Find("GameManager").GetComponent<TimeController>().dayNr;
        sleepArea = GameObject.Find("Bed").gameObject.transform.GetChild(0).gameObject.transform.Find("Collider").GetComponent<CapsuleController>();
    }
    /// <summary>
    /// If state is Bare, spawns seeds and changes state
    /// Saves type of planted seed
    /// </summary>
    /// <param name="t_plant"></param>
    public void plantSeeds(GameObject t_plant)
    {
        if (plantState == PlantState.Bare)
        {
            Vector3 pos = new Vector3(transform.position.x, 0.02625f, transform.position.z);
            Instantiate(seeds, pos, Quaternion.identity, gameObject.transform);
            plantState = PlantState.Seed;

            grownPlant = t_plant;
        }
    }
    /// <summary>
    /// Saves the passed in fully-grown fruit gameobject
    /// </summary>
    /// <param name="t_plant"></param>
    public void setFruitType(GameObject t_plant)
    {
        fruitType = t_plant;
    }
    /// <summary>
    /// Checks if player has been asleep, checks if watered
    /// If both true, changes plant state to next stage and spawns new plant gameobject
    /// Sets state to bare if all fruit harvested
    /// </summary>
    private void Update()
    {
        if (sleepArea.asleep)
        {
            if (plantState == PlantState.Seed && watered)
            {
                currentDay = GameObject.Find("GameManager").GetComponent<TimeController>().dayNr;
                plantState++;
                Debug.Log(plantState);
                Vector3 pos = gameObject.transform.GetChild(1).transform.position;
                Destroy(gameObject.transform.GetChild(1).gameObject);
                Instantiate(growingPlant, pos, Quaternion.identity, gameObject.transform);
            }
            else if (plantState == PlantState.Growing && watered)
            {
                currentDay = GameObject.Find("GameManager").GetComponent<TimeController>().dayNr;
                plantState++;
                Debug.Log(plantState);
                Vector3 pos = gameObject.transform.GetChild(1).transform.position;
                Destroy(gameObject.transform.GetChild(1).gameObject);
                Instantiate(grownPlant, pos, Quaternion.identity, gameObject.transform);
            }
            else if (plantState == PlantState.Grown && watered)
            {
                currentDay = GameObject.Find("GameManager").GetComponent<TimeController>().dayNr;
                plantState++;
                Debug.Log(plantState);
                Vector3 pos = gameObject.transform.GetChild(1).transform.position;
                pos.y = checkFruitYPos(fruitType.name);
                Destroy(gameObject.transform.GetChild(1).gameObject);
                Instantiate(fruitType, pos, Quaternion.identity, gameObject.transform);
            }
        }

        if (sleepArea.asleep)
        {
            gameObject.GetComponent<MeshRenderer>().material = dryMat;
            watered = false;
        }

        if (gameObject.transform.childCount == 1)
        {
            plantState = PlantState.Bare;
        }
    }
   
    /// <summary>
    /// Changes colour of field to show it watered
    /// </summary>
    public void makeWatered()
    {
        gameObject.GetComponent<MeshRenderer>().material = wetMat;
        watered = true;
    }

    /// <summary>
    /// Sets correct y-position depending on plant type
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private float checkFruitYPos(string name)
    {
        float value = gameObject.transform.GetChild(1).transform.position.y;

        if (name.Contains("Carrot"))
        {
            value = 0.1f;
        }
        else if(name.Contains("Turnip"))
        {
            value = 0.1f;
        }
        return value;
    }

    /// <summary>
    /// Checks for collisions with trigger objects
    /// Upon collision with grass, destroys grass
    /// Upon collision with static objects, destroys farm gameobject
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Grass")
        {
            Destroy(other.gameObject);
        }
        else if(other.tag == "Tree" || other.tag == "Building" || other.tag == "River" || other.gameObject.layer == LayerMask.NameToLayer("Decor"))
        {
            Debug.Log(other.name);

            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Checks for collisions with collider objects
    /// Upon collision with grass, destroys grass
    /// Upon collision with static objects, destroys farm gameobject
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Grass")
        {
            Destroy(collision.collider.gameObject);
        }
        else if(collision.collider.tag == "Tree" || collision.collider.tag == "Building" || collision.collider.tag == "River" || collision.collider.gameObject.layer == LayerMask.NameToLayer("Decor"))
        {
            Debug.Log(collision.collider.name);
            Destroy(this.gameObject);
        }
    }
}
