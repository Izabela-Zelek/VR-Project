using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SeedBagController : MonoBehaviour
{
    public string plantType;
    public GameObject seed;
    public InputActionProperty rightSelect;
    private XRDirectInteractor rightInteractor;
    private XRDirectInteractor leftInteractor;
    private int max_seed = 20;
    private int seed_count = 0;
    public string plotName;
    private bool pickedPlot = false;
    public Transform SpawnPoint;
    float timer = 0.5f;
    private PlantController plantController;

    private void Start()
    {
        plantController = GameObject.Find("GameManager").GetComponent<PlantController>();
        rightInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(2).GetComponent<XRDirectInteractor>();
        leftInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(1).GetComponent<XRDirectInteractor>();
    }
    private void Update()
    {
        if (seed_count >= max_seed - 1)
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

        if ((rightInteractor.interactablesSelected.Count > 0 && rightInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()) || (leftInteractor.interactablesSelected.Count > 0 && leftInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()))
        {
            if (other.tag == "SeedArea" && rightSelect.action.ReadValue<float>() >= 0.1f)
            {
                RaycastHit seen;
                Ray raydirection = new Ray(transform.position, transform.up);

                if (Physics.Raycast(raydirection, out seen, 5))
                {
                    if (seen.collider.tag == "Ground" || seen.collider.tag == "Plane")
                    {
                        if (!pickedPlot && other.gameObject.transform.parent.GetComponent<FarmScript>().plantState == PlantState.Bare)
                        {
                            float number = Random.Range(0.0f, 5.0f);
                            other.gameObject.transform.parent.name = other.gameObject.transform.parent.name + number.ToString();
                            plotName = other.gameObject.transform.parent.name;
                            pickedPlot = true;
                            Debug.Log(plotName);
                        }
                        if (other.gameObject.transform.parent.name == plotName)
                        {
                            if (seed_count < max_seed)
                            {
                                Instantiate(seed, SpawnPoint.position, Quaternion.identity);
                                seed_count++;
                            }
                            else
                            {
                                other.gameObject.transform.parent.GetComponent<FarmScript>().plantSeeds(plantController.getPlant(plantType));
                                other.gameObject.transform.parent.GetComponent<FarmScript>().setFruitType(plantController.getFruit(plantType));
                            }
                        }
                    }
                }
            }
        }
    }
}
