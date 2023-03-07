using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathMover : MonoBehaviour
{
    public List<Vector3> path;
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
    void Start()
    {
        if(id == -1)
        {
            id = Random.Range(0, 10000);
        }
        rb = GetComponent<Rigidbody>();
        //FindAnyObjectByType<PathFollowing>().OnNewPathCreated += SetPoints;
        SetPointsByChildren();
        yPos = transform.localPosition.y;

    }

    private void SetPoints(IEnumerable<Vector3> points)
    {
        path = (List<Vector3>)points;
        targetWaypoint = GetClosestPointOnPath(transform.position);
    }

    private void SetPointsByChildren()
    {
        GameObject pathObject = GameObject.Find("Path" + id);
        for(int i = 0;i < pathObject.transform.childCount;i++)
        {
            path.Add(pathObject.transform.GetChild(i).transform.position);
        }
        
        targetWaypoint = GetClosestPointOnPath(transform.position);
    }

    void Update()
    {
        if (path.Count > 0)
        {
            float distance = Vector3.Distance(transform.position, targetWaypoint);

            if (distance <= pathRadius)
            {
                if(pathForward)
                {
                    currentWaypointIndex++;
                }
                else if(!pathForward)
                {
                    currentWaypointIndex--;
                }
                if (currentWaypointIndex >= path.Count && pathForward) 
                {
                    pathForward = false;
                    currentWaypointIndex= path.Count - 1;
                }
                else if(currentWaypointIndex< 0 && !pathForward)
                {
                    pathForward = true;
                    currentWaypointIndex = 0;
                }
                targetWaypoint = path[currentWaypointIndex];
            }

            desiredVelocity = (targetWaypoint - transform.position).normalized * speed;
            steeringForce = desiredVelocity - rb.velocity;
            steeringForce /= mass;

            if (steeringForce.magnitude > maxSteer)
            {
                steeringForce = steeringForce.normalized * maxSteer;
            }

            rb.AddForce(steeringForce);
            Quaternion lookRotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
            //rb.velocity += steeringForce;
            transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);
        }
    }

    private Vector3 GetClosestPointOnPath(Vector3 position)
    {
        Vector3 closestPoint = Vector3.zero;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 pathPoint = path[i];
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
}