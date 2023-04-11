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
    private bool startPath = false;
    public int startWalkTime = 8;
    public int endWalkTime = 17;
    public bool first = false;
    public bool inside = false;
    void Start()
    {
        if (id == -1)
        {
            id = Random.Range(10, 10000);
        }
        rb = GetComponent<Rigidbody>();
        SetPointsByChildren(first);
        yPos = transform.localPosition.y;
        animator = GetComponent<Animator>();
        startWalkTime = Random.Range(6, 11);
        endWalkTime = Random.Range(16, 20);
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

    private void SetPointsByChildren(bool first)
    {
        if (id != -1 && path.Count == 0)
        {
            if(first)
            {
                GameObject pathObject = GameObject.Find("Path" + 0);
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

    void Update()
    {
        elapsedTime = Time.time - startTime;
        if (path.Count > 0 && !inside)
        {
            if (!startPath)
            {
                if (startWalkTime == GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.Hour)
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
                    if (path[currentWaypointIndex].GetComponent<PathCellController>().GetLoiterTime() > 0 && !hadLoiter)
                    {
                        rb.velocity = Vector3.zero;
                        animator.runtimeAnimatorController = Resources.Load("BasicMotions@Talk") as RuntimeAnimatorController;
                        isStopped = true;
                        StartCoroutine(Loiter(path[currentWaypointIndex].GetComponent<PathCellController>().GetLoiterTime()));
                    }

                    if(path[currentWaypointIndex].GetComponent<PathCellController>().GetAtShop())
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
                            if(first)
                            {
                                first = false;
                                path.Clear();
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
                    //rb.velocity += steeringForce;
                    transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);

                }
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

    public int GetStartTime()
    {
        return startWalkTime;
    }

    private void Hide()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

    }

    private void Unhide()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private IEnumerator LoiterInside(int time)
    {
        yield return new WaitForSeconds(time);
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        inside = false;
        Unhide();
    }
}
