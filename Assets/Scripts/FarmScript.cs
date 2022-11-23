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
    public GameObject turnipPlant;
    public Material dryMat;
    public Material wetMat;
    public PlantState plantState;
    private int currentDay;
    private bool watered = false;

    private ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];

    private void Start()
    {
        plantState = PlantState.Bare;
        currentDay = GameObject.Find("TimeController").GetComponent<TimeController>().dayNr;
    }
    public void plantSeeds()
    {
        if (plantState == PlantState.Bare)
        {
            Vector3 pos = new Vector3(transform.position.x, 0.02625f, transform.position.z);
            Instantiate(seeds, pos, Quaternion.identity, gameObject.transform);
            plantState = PlantState.Seed;
        }
    }
    private void Update()
    {
        if(GameObject.Find("TimeController").GetComponent<TimeController>().currentTime.ToString("HH:mm") == "01:00")
        {
            gameObject.GetComponent<MeshRenderer>().material = dryMat;
            watered = false;
            Debug.Log("NO NOWATER");

        }

        if (GameObject.Find("TimeController").GetComponent<TimeController>().currentTime.ToString("HH:mm") == "00:00")
        {
            if (GameObject.Find("TimeController").GetComponent<TimeController>().dayNr > currentDay && plantState == PlantState.Seed && watered)
            {
                currentDay = GameObject.Find("TimeController").GetComponent<TimeController>().dayNr;
                plantState++;
                Debug.Log(plantState);
                Vector3 pos = gameObject.transform.GetChild(1).transform.position;
                Destroy(gameObject.transform.GetChild(1).gameObject);
                Instantiate(growingPlant, pos, Quaternion.identity, gameObject.transform);
            }
            else if (GameObject.Find("TimeController").GetComponent<TimeController>().dayNr > currentDay && plantState == PlantState.Growing && watered)
            {
                currentDay = GameObject.Find("TimeController").GetComponent<TimeController>().dayNr;
                plantState++;
                Debug.Log(plantState);
                Vector3 pos = gameObject.transform.GetChild(1).transform.position;
                Destroy(gameObject.transform.GetChild(1).gameObject);
                Instantiate(turnipPlant, pos, Quaternion.identity, gameObject.transform);
            }
        }
    }
   
    public void makeWatered()
    {
        gameObject.GetComponent<MeshRenderer>().material = wetMat;
        watered = true;
    }
}
