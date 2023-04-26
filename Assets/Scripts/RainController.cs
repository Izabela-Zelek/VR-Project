using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
/// <summary>
/// Handles the water particle system that plays when the watering can is above a planting field
/// </summary>
public class RainController : MonoBehaviour
{
    private ParticleSystem water;

    private void Start()
    {
        water = GetComponent<ParticleSystem>();
        water.Stop();
    }
    /// <summary>
    /// Changes angle of water based on the angle of the parent
    /// </summary>
    void Update()
    {
        if (water.isPlaying)
        { 
            transform.eulerAngles = new Vector3(transform.parent.parent.eulerAngles.x + 90, transform.parent.parent.eulerAngles.y, transform.parent.parent.eulerAngles.z + 90); 
        }
    }

    public void turnOnWater()
    {
        water.Play();
    }

    public void turnOffWater()
    {
        water.Stop();
    }
}
