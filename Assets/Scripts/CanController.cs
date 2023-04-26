using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the functionality of the watering can
/// </summary>
public class CanController : MonoBehaviour
{
    public InputActionProperty rightSelect;

    private float waterTime = 2.0f;
    //private void Update()
    //{
    //    Debug.DrawRay(transform.GetChild(1).transform.position, transform.up + transform.forward, Color.black, 1);
    //}

    /// <summary>
    /// Checks for collision with the boundary box of a planting field
    /// Plays water particle system upon collision
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //RaycastHit seen;
        //Ray raydirection = new Ray(transform.GetChild(1).transform.position, transform.up + transform.forward );
        //if (Physics.Raycast(raydirection, out seen, 5))
        //{
            if (other.tag == "SeedArea" && rightSelect.action.ReadValue<float>() >= 0.1f)
            {
                transform.GetChild(1).transform.GetChild(0).GetComponent<RainController>().turnOnWater();
            }
            //else
            //{
            //    transform.GetChild(1).transform.GetChild(0).GetComponent<RainController>().turnOffWater();
            //}
        //}
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
        if (other.tag == "SeedArea" && rightSelect.action.ReadValue<float>() >= 0.1f)
        {
            waterTime -= Time.deltaTime;
            Debug.Log(waterTime);
            if (waterTime <= 0)
            { 
                other.transform.parent.GetComponent<FarmScript>().makeWatered(); 
            }
        }
        else if(other.tag == "SeedArea" && rightSelect.action.ReadValue<float>() < 0.1f)
        {
            transform.GetChild(1).transform.GetChild(0).GetComponent<RainController>().turnOffWater();
            waterTime = 2.0f;
        }
    }
}
