using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles catching objects which fall out of world border and sets position of object to ground level
/// </summary>
public class FallController : MonoBehaviour
{
    /// <summary>
    /// Upon collision with trigger objects, changes position to ground level
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = new Vector3(other.transform.position.x, 1, other.transform.position.z);
    }

    /// <summary>
    ///  Upon collision with collider objects, changes position to ground level
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        collision.transform.position = new Vector3(collision.transform.position.x, 1, collision.transform.position.z);

    }
}
