using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Handles the path following steering behaviour of shopkeeper NPCs
/// </summary>
public class ShopKeeperMover : MonoBehaviour
{
    public List<GameObject> path;
    public float speed = 5.0f;
    public float mass = 5.0f;
    public float maxSteer = 15.0f;
    public float pathRadius = 0.001f;
    public int currentWaypointIndex = 0;
    private Animator animator;

    private bool isStopped = false;
    private Vector3 targetWaypoint;
    private Vector3 desiredVelocity;
    private Vector3 steeringForce;
    private Rigidbody rb;
    private float yPos;
    private bool pathForward = true;
    public int id = -1;
    private bool onGround = true;

    public bool canChangeY = true;
    private float startTime;
    private float duration = 10.0f;
    private float elapsedTime;
    private bool startPath = false;
    public int StartWalkTime = 4;
    private int endWalkTime = 17;
    private bool working = false;
    public int ShopRotation = 0;
    public int dayOff = 0;

    /// <summary>
    /// Calls for function to set up initial path
    /// </summary>
    void Start()
    { 
        rb = GetComponent<Rigidbody>();
        SetPointsByChildren();
        yPos = transform.localPosition.y;
        animator = GetComponent<Animator>();
        startTime = Time.time;
    }

    /// <summary>
    /// Finds the path gameobject with the assigned path id and adds all children positions to a list
    /// Calls for function to find the closest position in that list
    /// Increments path by one as the first cell is the house doorstep
    /// </summary>
    private void SetPointsByChildren()
    {
        if (id != -1 && path.Count == 0)
        {
            GameObject pathObject = GameObject.Find("Path" + id);
            for (int i = 0; i < pathObject.transform.childCount; i++)
            {
                path.Add(pathObject.transform.GetChild(i).gameObject);
            }

            targetWaypoint = GetClosestPointOnPath(transform.position);
            currentWaypointIndex++;
            targetWaypoint = path[currentWaypointIndex].transform.position;
        }
    }

    /// <summary>
    /// If the NPC is outside and they have a path, they will start walking that path
    /// Starting from the previously calculated closest position, once the NPC position is within bounds of the next position in the list, they current waypoint gets incremented
    /// Changes animation based on whether they are walking or attending their shop
    /// Iterates backwards through path once the current world time is equal to their sleep time
    /// Adds force to gameobject to move in the direction of the next point - with smoothing
    /// </summary>
    void Update()
    {
        elapsedTime = Time.time - startTime;
        if (path.Count > 0 && !working)
        {
            if(!startPath)
            {
                if (StartWalkTime == GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.Hour && WorkToday())
                {
                    startPath = true;
                    if (animator.runtimeAnimatorController.name != "BasicMotions@Walk")
                    {
                        animator.runtimeAnimatorController = Resources.Load("BasicMotions@Walk") as RuntimeAnimatorController;
                    }
                }
            }
            if (startPath)
            {
                float distance = Vector3.Distance(transform.position, targetWaypoint);

                if (distance <= pathRadius)
                {
                    if (!isStopped)
                    {
                        if (animator.runtimeAnimatorController.name != "BasicMotions@Walk")
                        {
                            animator.runtimeAnimatorController = Resources.Load("BasicMotions@Walk") as RuntimeAnimatorController;
                        }
                        if (currentWaypointIndex >= path.Count - 1 && pathForward)
                        {
                            working = true;
                            pathForward = false;
                            currentWaypointIndex = path.Count - 1;
                        }
                        if (pathForward && !working)
                        {
                            currentWaypointIndex++;
                        }
                        else if (!pathForward && !working)
                        {
                            currentWaypointIndex--;
                        }
                        
                       if (currentWaypointIndex < 0 && !pathForward && !working)
                        {
                            pathForward = true;
                            currentWaypointIndex = 1;
                            gameObject.SetActive(false);
                        }
                        if (!working)
                        { 
                            targetWaypoint = path[currentWaypointIndex].transform.position; 
                        }
                    }

                }

                if (!isStopped)
                {
                    desiredVelocity = (targetWaypoint - transform.position).normalized * speed;
                    steeringForce = desiredVelocity - rb.velocity;
                    steeringForce /= mass;

                    if (steeringForce.magnitude > maxSteer)
                    {
                        steeringForce = steeringForce.normalized * maxSteer;
                    }

                    rb.AddForce(steeringForce);
                    if (rb.velocity != Vector3.zero)
                    {
                        Quaternion lookRotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
                    }
                    transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);

                }
            }
        }
        else if(working)
        {
            animator.runtimeAnimatorController = Resources.Load("BasicMotions@Idle") as RuntimeAnimatorController;
            transform.rotation = Quaternion.Euler(0f, ShopRotation, 0f);
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }

        if (endWalkTime == GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.Hour)
        {
            working = false;
            startPath = true;
            if (animator.runtimeAnimatorController.name != "BasicMotions@Walk")
            {
                animator.runtimeAnimatorController = Resources.Load("BasicMotions@Walk") as RuntimeAnimatorController;
            }
            rb.constraints = RigidbodyConstraints.FreezePositionY;
        }
    }

    /// <summary>
    /// Iterates through list of positions and finds the position closest to the NPC
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Checks for collisions with curb objects to boost the NPC up or down curbs
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (elapsedTime >= duration)
        {
            startTime = Time.time;
            canChangeY = true;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Curb") && canChangeY)
        {
            if(onGround)
            {
                yPos = transform.localPosition.y + 0.05f;
                onGround = false;
            }
           else
            {
                yPos = transform.localPosition.y - 0.05f;
                onGround = false;
            }
            canChangeY = false;
        }
    }

    public int GetStartTime()
    {
        return StartWalkTime;
    }
    /// <summary>
    /// Returns whether or not the NPC is supposed to be working on the current day
    /// </summary>
    /// <returns></returns>
    public bool WorkToday()
    {
        return GameObject.Find("GameManager").GetComponent<TimeController>().GetDayOfWeek() != dayOff;
    }
}
