using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class RainController : MonoBehaviour
{
    private ParticleSystem water;

    private void Start()
    {
        water = GetComponent<ParticleSystem>();
        water.Stop();
    }
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

    void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Farm")
        {
            other.GetComponent<FarmScript>().makeWatered();
        }
    }
}
