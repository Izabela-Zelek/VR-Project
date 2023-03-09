using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAvoidance : MonoBehaviour
{
    private Vector3 avoidance_force;
    private float max_avoidance = 60;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "NPCCollision")
        {
            if (transform.parent.GetComponent<Rigidbody>().velocity != Vector3.zero)
            {
                avoidance_force = this.transform.position - other.transform.position;
                avoidance_force = avoidance_force.normalized * max_avoidance;

                transform.parent.GetComponent<NPCContoller>().Avoid(avoidance_force);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "NPCCollision")
        {
            if (transform.parent.GetComponent<Rigidbody>().velocity != Vector3.zero)
            {
                avoidance_force = this.transform.position - collision.transform.position;
                avoidance_force = avoidance_force.normalized * max_avoidance;

                transform.parent.GetComponent<NPCContoller>().Avoid(avoidance_force);
            }
        }
    }
}
