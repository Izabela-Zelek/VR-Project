using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAvoidance : MonoBehaviour
{
    private Vector3 avoidance_force;
    private float max_avoidance = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (GetComponent<Rigidbody>().velocity != Vector3.zero)
        {
            avoidance_force = this.transform.position - GetComponent<Collider>().transform.position;
            avoidance_force = avoidance_force.normalized * max_avoidance;

            transform.parent.GetComponent<NPCContoller>().Avoid(avoidance_force);
        }
    }
}
