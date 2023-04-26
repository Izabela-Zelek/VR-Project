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
    public string PlantType;
    public GameObject Seed;
    public InputActionProperty RightSelect;
    public string PlotName;
    public Transform SpawnPoint;

    private XRDirectInteractor _rightInteractor;
    private XRDirectInteractor _leftInteractor;
    private int _maxSeed = 20;
    private int _seedCount = 0;
    private bool _pickedPlot = false;
    private float _timer = 0.5f;
    private PlantController _plantController;
    private AudioSource _seedsFall;

    private void Start()
    {
        _plantController = GameObject.Find("GameManager").GetComponent<PlantController>();
        _rightInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(2).GetComponent<XRDirectInteractor>();
        _leftInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(1).GetComponent<XRDirectInteractor>();
        _seedsFall = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (_seedCount >= _maxSeed - 1)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
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

        if ((_rightInteractor.interactablesSelected.Count > 0 && _rightInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()) || (_leftInteractor.interactablesSelected.Count > 0 && _leftInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()))
        {
            if (other.tag == "SeedArea" && RightSelect.action.ReadValue<float>() >= 0.1f)
            {

                if ((transform.rotation.eulerAngles.x >= 105 && transform.rotation.eulerAngles.x <= 255) || (transform.rotation.eulerAngles.x <= -105 && transform.rotation.eulerAngles.x >= -255) ||
                    (transform.rotation.eulerAngles.z >= 105 && transform.rotation.eulerAngles.z <= 255) || (transform.rotation.eulerAngles.z <= -105 && transform.rotation.eulerAngles.z >= -255))
                {
                    if (!_pickedPlot && other.gameObject.transform.parent.GetComponent<FarmScript>().PlantState == PlantState.Bare)
                    {
                        float number = Random.Range(0.0f, 5.0f);
                        other.gameObject.transform.parent.name = other.gameObject.transform.parent.name + number.ToString();
                        PlotName = other.gameObject.transform.parent.name;
                        _pickedPlot = true;
                    }
                    if (other.gameObject.transform.parent.name == PlotName)
                    {
                        if (!_seedsFall.isPlaying)
                        {
                            _seedsFall.Play();
                        }
                        if (_seedCount < _maxSeed)
                        {
                            Instantiate(Seed, SpawnPoint.position, Quaternion.identity);
                            _seedCount++;
                        }
                        else
                        {
                            other.gameObject.transform.parent.GetComponent<FarmScript>().PlantSeeds(_plantController.GetPlant(PlantType));
                            other.gameObject.transform.parent.GetComponent<FarmScript>().SetFruitType(_plantController.GetFruit(PlantType));
                        }
                    }
                }
            }
        }
    }
}
