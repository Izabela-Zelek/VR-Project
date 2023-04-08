using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathMover : MonoBehaviour
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
    void Start()
    {
        if (id == -1)
        {
            id = Random.Range(10, 10000);
        }
        rb = GetComponent<Rigidbody>();
        //FindAnyObjectByType<PathFollowing>().OnNewPathCreated += SetPoints;
        SetPointsByChildren();
        yPos = transform.localPosition.y;
        animator = GetComponent<Animator>();
        startTime = Time.time;
    }

    public void SetDefaultPath(int newId)
    {
        path.Clear();
        id = newId;
        GameObject pathObject = GameObject.Find("Path" + id);
        for (int i = 0; i < pathObject.transform.childCount; i++)
        {
            path.Add(pathObject.transform.GetChild(i).gameObject);
        }

        targetWaypoint = GetClosestPointOnPath(transform.position);
        transform.LookAt(new Vector3(targetWaypoint.x, 0, targetWaypoint.z));
    }

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
        }
    }

    void Update()
    {
        elapsedTime = Time.time - startTime;
        if (path.Count > 0)
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
                if (!isStopped)
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
                    if (currentWaypointIndex >= path.Count && pathForward)
                    {
                        pathForward = false;
                        currentWaypointIndex = path.Count - 1;
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
                if(Quaternion.LookRotation(rb.velocity, Vector3.up) != new Quaternion(0,0,0,1))
                {
                    Quaternion lookRotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
                }
                //rb.velocity += steeringForce;
                transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);

            }
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

    private IEnumerator Loiter(int time)
    {
        yield return new WaitForSeconds(time);
        isStopped = false;
        hadLoiter = true;
    }


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
}
