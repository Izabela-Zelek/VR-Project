using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the destruction of the falling seed when it hits the farm ground
/// </summary>
public class SeedScript : MonoBehaviour
{    
    /// <summary>
     /// Upon collision with ground, destroys seed gameobject.
     /// </summary>
     /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Farm")
        {
            Destroy(gameObject);
        }
    }
}
