using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAvoidance : MonoBehaviour
{
    private Vector3 avoidance_force;
    private float max_avoidance = 90;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "NPCCollision" && other.tag != "Ground" && other.tag != "Plane" && other.gameObject.layer != LayerMask.NameToLayer("Curb"))
        {
            if (transform.parent.GetComponent<Rigidbody>().velocity != Vector3.zero)
            {
                //avoidance_force = this.transform.position - other.transform.position;
                //avoidance_force = avoidance_force.normalized * max_avoidance;

                avoidance_force = Vector3.Reflect(transform.forward, other.transform.position).normalized;
                avoidance_force = avoidance_force.normalized * max_avoidance;

                transform.parent.GetComponent<NPCContoller>().Avoid(avoidance_force);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "NPCCollision" && collision.collider.tag != "Ground" && collision.collider.tag != "Plane" && collision.gameObject.layer != LayerMask.NameToLayer("Curb"))
        {
            Debug.Log(collision.collider.name);
            if (transform.parent.GetComponent<Rigidbody>().velocity != Vector3.zero)
            {
                //avoidance_force = this.transform.position - collision.transform.position;
                //avoidance_force = avoidance_force.normalized * max_avoidance;

                avoidance_force = Vector3.Reflect(transform.forward, collision.transform.position).normalized;
                avoidance_force = avoidance_force.normalized * max_avoidance;

                transform.parent.GetComponent<NPCContoller>().Avoid(avoidance_force);
            }
        }
    }
}
