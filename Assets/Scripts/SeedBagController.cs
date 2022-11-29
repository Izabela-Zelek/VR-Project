using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class SeedBagController : MonoBehaviour
{
    public string plantType;
    public GameObject seed;
    public InputActionProperty rightSelect;
    private int max_seed = 20;
    private int seed_count = 0;
    private string plotName;
    private bool pickedPlot = false;
    public Transform SpawnPoint;
    float timer = 0.5f;
    private PlantController plantController = new PlantController();

    private void Start()
    {
        plantController = GameObject.Find("GameManager").GetComponent<PlantController>();
    }
    private void Update()
    {
        if(seed_count >= max_seed - 1)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            { 
                Destroy(gameObject); 
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "SeedArea" && rightSelect.action.ReadValue<float>() >= 0.1f)
        {
            if (!pickedPlot && other.gameObject.transform.parent.GetComponent<FarmScript>().plantState == PlantState.Bare)
            {
                float number = Random.Range(0.0f, 5.0f);
                other.gameObject.transform.parent.name = other.gameObject.transform.parent.name + number.ToString(); 
                plotName = other.gameObject.transform.parent.name;
                pickedPlot = true;
            }
            if (other.gameObject.transform.parent.name == plotName)
            {
                RaycastHit seen;
                Ray raydirection = new Ray(transform.position, transform.up);
                if (Physics.Raycast(raydirection, out seen, 5))
                {
                    if (seen.collider.tag == "Ground")
                    {
                        if (seed_count < max_seed)
                        {
                            Instantiate(seed, SpawnPoint.position, Quaternion.identity);
                            seed_count++;
                        }
                        else
                        {
                            other.gameObject.transform.parent.GetComponent<FarmScript>().plantSeeds(plantController.getPlant(plantType));
                        }
                    }

                }
            }
        }
    }
}
