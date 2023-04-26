using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the functionality of the watering can
/// </summary>
public class CanController : MonoBehaviour
{
    public InputActionProperty RightSelect;

    private float waterTime = 2.0f;

    /// <summary>
    /// Checks for collision with the boundary box of a planting field
    /// Plays water particle system upon collision
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SeedArea" && RightSelect.action.ReadValue<float>() >= 0.1f)
        {
            transform.GetChild(1).transform.GetChild(0).GetComponent<RainController>().turnOnWater();
        }
    }

    /// <summary>
    /// Checks for end of collision with the boundary box of a planting field
    /// Stops water particle system upon end
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SeedArea")
        {
            transform.GetChild(1).transform.GetChild(0).GetComponent<RainController>().turnOffWater();
            waterTime = 2.0f;
        }
    }

    /// <summary>
    /// Starts countdown while in collision with boundary box of planting field
    /// Upon time over, changes state of field to watered
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "SeedArea" && RightSelect.action.ReadValue<float>() >= 0.1f)
        {
            waterTime -= Time.deltaTime;
            Debug.Log(waterTime);
            if (waterTime <= 0)
            { 
                other.transform.parent.GetComponent<FarmScript>().makeWatered(); 
            }
        }
        else if(other.tag == "SeedArea" && RightSelect.action.ReadValue<float>() < 0.1f)
        {
            transform.GetChild(1).transform.GetChild(0).GetComponent<RainController>().turnOffWater();
            waterTime = 2.0f;
        }
    }
}
