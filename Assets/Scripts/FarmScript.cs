using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public enum PlantState
{
    Bare = 0,
    Seed = 1,
    Growing = 2,
    Grown = 3,
    Fruit = 4
}
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

    private void Start()
    {
        plantState = PlantState.Bare;
        currentDay = GameObject.Find("GameManager").GetComponent<TimeController>().dayNr;
        sleepArea = GameObject.Find("Bed").gameObject.transform.GetChild(0).gameObject.transform.Find("Collider").GetComponent<CapsuleController>();
    }
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
    public void setFruitType(GameObject t_plant)
    {
        fruitType = t_plant;
    }
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

                //USED FOR TESTING

                //currentDay = GameObject.Find("GameManager").GetComponent<TimeController>().dayNr;
                //plantState++;
                //Debug.Log(plantState);
                //Vector3 pos = gameObject.transform.GetChild(1).transform.position;
                //pos.y = checkFruitYPos(fruitType.name);
                //Destroy(gameObject.transform.GetChild(1).gameObject);
                //Instantiate(fruitType, pos, Quaternion.identity, gameObject.transform);
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
            Debug.Log("NO NOWATER");

        }

        if (gameObject.transform.childCount == 1)
        {
            plantState = PlantState.Bare;
        }
    }
   
    public void makeWatered()
    {
        gameObject.GetComponent<MeshRenderer>().material = wetMat;
        watered = true;
    }

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
}
