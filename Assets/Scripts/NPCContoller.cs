using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCContoller : MonoBehaviour
{
    public float circleRadius = 100;
    public float distance = 40;

    private Rigidbody rb;
    private float yPos;
    public float wanderWeight = 10.0f;
    public float maxSpeed = 8;
    public float maxForce = 10;
    private float angle;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        yPos = transform.localPosition.y;
    }

    private void FixedUpdate()
    {
        rb.AddForce(Wander() * wanderWeight);

        Quaternion lookRotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
        transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);
    }

    private Vector3 Wander()
    {
        angle = angle + Random.Range(-20, 20) * Mathf.Deg2Rad;

        Vector3 futurePos = transform.position + rb.velocity * distance;
        Vector3 pointOnCircle = futurePos;
        pointOnCircle.x = pointOnCircle.x + (circleRadius * Mathf.Cos(angle));
        pointOnCircle.z = pointOnCircle.z + (circleRadius * Mathf.Sin(angle));

        Vector3 desVelocity = (pointOnCircle - transform.position).normalized * maxSpeed;

        Vector3 steer = desVelocity - rb.velocity;

        if (steer.magnitude > maxForce)
        {
            steer = steer.normalized * maxForce;
        }

        return steer;
    }

    private void SimpleWander()
    {
        angle = angle + Random.Range(-180, 180) * Mathf.Deg2Rad;
    }

    public void Avoid(Vector3 force)
    {
        rb.AddForce(force);
    }
}
