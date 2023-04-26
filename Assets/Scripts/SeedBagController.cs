using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
/// <summary>
/// Handles the interaction with the seed bag- Instantiates seeds when the bag is tipped over the planting field
/// </summary>
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
    private AudioSource _seedsFall;
    private void Start()
    {
        plantController = GameObject.Find("GameManager").GetComponent<PlantController>();
        rightInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(2).GetComponent<XRDirectInteractor>();
        leftInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(1).GetComponent<XRDirectInteractor>();
        _seedsFall = GetComponent<AudioSource>();
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
    /// <summary>
    /// If the seedbag collides with the planting field collider and the angle is correct, spawns seed falling out of bag and hitting ground.
    /// Seed bags can only be used in one planting field- Upon starting to pour seeds, the planting field below is saved and checked when next pouring.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {

        if ((rightInteractor.interactablesSelected.Count > 0 && rightInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()) || (leftInteractor.interactablesSelected.Count > 0 && leftInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()))
        {
            if (other.tag == "SeedArea" && rightSelect.action.ReadValue<float>() >= 0.1f)
            {

                if ((transform.rotation.eulerAngles.x >= 105 && transform.rotation.eulerAngles.x <= 255) || (transform.rotation.eulerAngles.x <= -105 && transform.rotation.eulerAngles.x >= -255) ||
                    (transform.rotation.eulerAngles.z >= 105 && transform.rotation.eulerAngles.z <= 255) || (transform.rotation.eulerAngles.z <= -105 && transform.rotation.eulerAngles.z >= -255))
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
                        if (!_seedsFall.isPlaying)
                        {
                            _seedsFall.Play();
                        }
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
