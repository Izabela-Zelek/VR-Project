using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Handles the path following steering behaviour of visiting NPCs
/// </summary>
public class ParkerPathMover : MonoBehaviour
{
    public List<GameObject> path;
    public float speed = 5.0f;
    public float mass = 5.0f;
    public float maxSteer = 15.0f;
    public float pathRadius = 1.0f;
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

    public bool hadLoiter = false;
    public bool canChangeY = true;
    private float startTime;
    private float duration = 10.0f;
    private float elapsedTime;
    public bool first = false;
    public bool inside = false;
    /// <summary>
    /// Calls for function to set up initial path
    /// </summary>
    private void Start()
    {
        if (id == -1)
        {
            id = Random.Range(1, 8);
        }
        rb = GetComponent<Rigidbody>();
        SetPointsByChildren(first);
        yPos = 0.1f;
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Finds the path gameobject with the assigned path id and adds all children positions to a list
    /// Calls for function to find the closest position in that list
    /// </summary>
    /// <param name="first"></param>
    private void SetPointsByChildren(bool first)
    {
        if (id != -1 && path.Count == 0)
        {
            if (first)
            {
                GameObject pathObject = GameObject.Find("Path" + 0.1);
                for (int i = 0; i < pathObject.transform.childCount; i++)
                {
                    path.Add(pathObject.transform.GetChild(i).gameObject);
                }

            }
            else
            {
                GameObject pathObject = GameObject.Find("Path" + id);
                for (int i = 0; i < pathObject.transform.childCount; i++)
                {
                    path.Add(pathObject.transform.GetChild(i).gameObject);
                }

            }

            targetWaypoint = GetClosestPointOnPath(transform.position);
        }
    }

    /// <summary>
    /// If the NPC is outside and they have a path, they will start walking that path
    /// Starting from the previously calculated closest position, once the NPC position is within bounds of the next position in the list, they current waypoint gets incremented
    /// Checks current position cell for loiter value, starts loiter coroutine if greater than 0
    /// Checks current position cell for bool determining if its inside a shop, hides NPC if so
    /// Changes animation based on whether they are walking or loitering/inside
    /// Assigns a new random path once NPC reaches the end of current one
    /// Adds force to gameobject to move in the direction of the next point - with smoothing
    /// </summary>
    private void Update()
    {
        elapsedTime = Time.time - startTime;
        if (path.Count > 0 && !inside)
        {
            float distance = Vector3.Distance(transform.position, targetWaypoint);

            if (distance <= pathRadius)
            {
                if (path[currentWaypointIndex].GetComponent<PathCellController>().GetLoiterTime() > 0 && !hadLoiter)
                {
                    rb.velocity = Vector3.zero;
                    animator.runtimeAnimatorController = Resources.Load("BasicMotions@Talk") as RuntimeAnimatorController;
                    isStopped = true;
                    StartCoroutine(Loiter(path[currentWaypointIndex].GetComponent<PathCellController>().GetLoiterTime()));
                }

                if (path[currentWaypointIndex].GetComponent<PathCellController>().GetAtShop())
                {
                    Hide();
                    int hideFor = Random.Range(9, 20);
                    StartCoroutine(LoiterInside(hideFor));
                }
                if (!isStopped && !inside)
                {
                    if (animator.runtimeAnimatorController.name != "BasicMotions@Walk")
                    {
                        animator.runtimeAnimatorController = Resources.Load("BasicMotions@Walk") as RuntimeAnimatorController;
                        hadLoiter = false;
                    }
                    if (pathForward)
                    {
                        currentWaypointIndex++;
                        hadLoiter = false;
                    }
                    else if (!pathForward)
                    {
                        currentWaypointIndex--;
                        hadLoiter = false;
                    }
                    if (currentWaypointIndex >= path.Count - 1 && pathForward)
                    {
                        if (first)
                        {
                            first = false;
                            path.Clear();
                            id = Random.Range(1, 8);
                            SetPointsByChildren(first);
                        }
                        else
                        {
                            pathForward = false;
                            currentWaypointIndex = path.Count - 1;
                        }
                    }
                    else if (currentWaypointIndex < 0 && !pathForward)
                    {
                        pathForward = true;
                        currentWaypointIndex = 0;
                    }
                    targetWaypoint = path[currentWaypointIndex].transform.position;
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
                transform.position = new Vector3(transform.position.x, yPos, transform.position.z);

            }

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
    /// Coroutine which allows the NPC to loiter at a particular position for an passed in amount of time
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator Loiter(int time)
    {
        yield return new WaitForSeconds(time);
        isStopped = false;
        hadLoiter = true;
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
            if (onGround)
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
    /// <summary>
    /// Disables the NPC model when entering a shop
    /// </summary>
    private void Hide()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

    }
    /// <summary>
    /// Enables the NPC model when leaving a shop
    /// </summary>
    private void Unhide()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// Coroutine which allows the NPC to loiter at a shop for an passed in amount of time
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator LoiterInside(int time)
    {
        yield return new WaitForSeconds(time);
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        inside = false;
        Unhide();
    }
}