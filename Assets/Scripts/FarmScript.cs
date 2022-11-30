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
    public Material dryMat;
    public Material wetMat;
    public PlantState plantState;
    private int currentDay;
    private bool watered = false;

    private ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];

    private void Start()
    {
        plantState = PlantState.Bare;
        currentDay = GameObject.Find("GameManager").GetComponent<TimeController>().dayNr;
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
    private void Update()
    {
        if(GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.ToString("HH:mm") == "01:00")
        {
            gameObject.GetComponent<MeshRenderer>().material = dryMat;
            watered = false;
            Debug.Log("NO NOWATER");

        }

        if (GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.ToString("HH:mm") == "00:00")
        {
            if (GameObject.Find("GameManager").GetComponent<TimeController>().dayNr > currentDay && plantState == PlantState.Seed && watered)
            {
                currentDay = GameObject.Find("GameManager").GetComponent<TimeController>().dayNr;
                plantState++;
                Debug.Log(plantState);
                Vector3 pos = gameObject.transform.GetChild(1).transform.position;
                Destroy(gameObject.transform.GetChild(1).gameObject);
                Instantiate(growingPlant, pos, Quaternion.identity, gameObject.transform);
            }
            else if (GameObject.Find("GameManager").GetComponent<TimeController>().dayNr > currentDay && plantState == PlantState.Growing && watered)
            {
                currentDay = GameObject.Find("GameManager").GetComponent<TimeController>().dayNr;
                plantState++;
                Debug.Log(plantState);
                Vector3 pos = gameObject.transform.GetChild(1).transform.position;
                Destroy(gameObject.transform.GetChild(1).gameObject);
                Instantiate(grownPlant, pos, Quaternion.identity, gameObject.transform);
            }
        }
    }
   
    public void makeWatered()
    {
        gameObject.GetComponent<MeshRenderer>().material = wetMat;
        watered = true;
    }
}
