using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleMover : MonoBehaviour
{
    public List<GameObject> path;
    public float speed = 5.0f;
    public float mass = 5.0f;
    public float maxSteer = 15.0f;
    public float pathRadius = 1.0f;

    public int currentWaypointIndex = 0;
    private Vector3 targetWaypoint;
    private Vector3 desiredVelocity;
    private Vector3 steeringForce;
    private Rigidbody rb;
    private float yPos;
    private bool pathForward = true;
    public int id = -1;

    private GameObject traffic;

    private bool canMove = true;
    void Start()
    {

        if (id == -1)
        {
            id = Random.Range(0, 10000);
        }
        rb = GetComponent<Rigidbody>();
        yPos = transform.localPosition.y;
        traffic = GameObject.Find("Traffic").gameObject;
    }

    public void SetPointsByChildren(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            path.Add(parent.transform.GetChild(i).gameObject);
        }

        targetWaypoint = GetClosestPointOnPath(transform.position);
    }

    void Update()
    {
        if (path.Count > 0 && canMove)
        {
            float distance = Vector3.Distance(transform.position, targetWaypoint);

            if (distance <= pathRadius)
            {
                if (!path[currentWaypointIndex].GetComponent<RoadCellController>().InFrontOfLight ||
                    (path[currentWaypointIndex].GetComponent<RoadCellController>().InFrontOfLight &&
                    path[currentWaypointIndex].GetComponent<RoadCellController>().TrafficLightInFront == traffic.GetComponent<TrafficLightController>().ReturnGreen1()) ||
                     (path[currentWaypointIndex].GetComponent<RoadCellController>().InFrontOfLight &&
                    path[currentWaypointIndex].GetComponent<RoadCellController>().TrafficLightInFront == traffic.GetComponent<TrafficLightController>().ReturnGreen2()))
                {
                    currentWaypointIndex++;

                    if (currentWaypointIndex >= path.Count && pathForward)
                    {
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        targetWaypoint = path[currentWaypointIndex].transform.position;
                    }
                }
                else
                {
                    rb.velocity = Vector3.zero;
                }
            }

            desiredVelocity = (targetWaypoint - transform.position).normalized * speed;
            steeringForce = desiredVelocity - rb.velocity;
            steeringForce /= mass;

            if (steeringForce.magnitude > maxSteer)
            {
                steeringForce = steeringForce.normalized * maxSteer;
            }

            rb.AddForce(steeringForce);
            Vector3 vel = new Vector3(rb.velocity.x, 0, rb.velocity.z); 

            if(Quaternion.LookRotation(vel) != new Quaternion(0,0,0,1))
            {
                transform.rotation = Quaternion.LookRotation(vel);
                transform.rotation *= Quaternion.Euler(0f, -90f, 0f);
            }

            transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    private Vector3 GetClosestPointOnPath(Vector3 position)
    {
        Vector3 closestPoint = Vector3.zero;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 pathPoint = path[i].transform.position;
            float distance = Vector3.Distance(position, pathPoint);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = pathPoint;
                currentWaypointIndex = i;
            }
        }

        return closestPoint;
    }

    public void setMove(bool move)
    {
        canMove = move;
    }
}