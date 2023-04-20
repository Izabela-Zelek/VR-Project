using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = new Vector3(other.transform.position.x, 1, other.transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.transform.position = new Vector3(collision.transform.position.x, 1, collision.transform.position.z);

    }
}
