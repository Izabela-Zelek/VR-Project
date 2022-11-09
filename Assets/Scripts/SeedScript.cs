using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedScript : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Farm")
        {
            Destroy(gameObject);
        }
    }
}
