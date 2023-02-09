using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCContoller : MonoBehaviour
{
    private float circleRadius = 100;
    private float distance = 40;

    private Rigidbody rb;

    public float wanderWeight = 10.0f;
    public float maxSpeed = 8;
    public float maxForce = 10;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        rb.AddForce(Wander() * wanderWeight);

        Quaternion lookRotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
    }

    private Vector3 Wander()
    {
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;

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
}
