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
    public bool late = false;
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
        startWalkTime = Random.Range(7, 13);
        endWalkTime = Random.Range(17, 21);
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

        if (id == 1)
        {
            for (int i = pathObject.transform.childCount - 1; i > 0; i--)
            {
                path.Add(pathObject.transform.GetChild(i).gameObject);
            }
        }

        targetWaypoint = GetClosestPointOnPath(transform.position);
        transform.LookAt(new Vector3(targetWaypoint.x, 0, targetWaypoint.z));
    }

    private void SetPointsByChildren(bool first)
    {
        if (id != -1 && path.Count == 0)
        {
            if (late)
            {
                GameObject pathObject = GameObject.Find("Path" + 0.5);
                if (transform.localPosition.z <= 12)
                {
                    string[] nameSplit = name.Split(new string[] { "Dude" }, System.StringSplitOptions.None);
                    int npcNr = int.Parse(nameSplit[1]);
                    for (int i = 0; i < pathObject.transform.childCount; i++)
                    {
                        if (!pathObject.transform.GetChild(i).name.Contains("Cell1"))
                        {
                            if (pathObject.transform.GetChild(i).name.Contains("Cell"))
                            {
                                path.Add(pathObject.transform.GetChild(i).gameObject);
                            }
                            else if (pathObject.transform.GetChild(i).name.Contains("MeetingPoint"))
                            {
                                path.Add(pathObject.transform.GetChild(i).gameObject);
                            }
                            else if (pathObject.transform.GetChild(i).name == npcNr.ToString())
                            {
                                path.Add(pathObject.transform.GetChild(i).gameObject);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    string[] nameSplit = name.Split(new string[] { "Dude" }, System.StringSplitOptions.None);
                    int npcNr = int.Parse(nameSplit[1]);
                    for (int i = 9; i < pathObject.transform.childCount; i++)
                    {
                        if (pathObject.transform.GetChild(i).name.Contains("Cell1"))
                        {
                            path.Add(pathObject.transform.GetChild(i).gameObject);
                        }
                        else if (pathObject.transform.GetChild(i).name.Contains("MeetingPoint"))
                        {
                            path.Add(pathObject.transform.GetChild(i).gameObject);
                        }
                        else if (pathObject.transform.GetChild(i).name == npcNr.ToString())
                        {
                            path.Add(pathObject.transform.GetChild(i).gameObject);
                            break;
                        }
                    }
                }
            }
            else if (first)
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

                if (id == 1)
                {
                    for (int i = pathObject.transform.childCount - 1; i > 0; i--)
                    {
                        path.Add(pathObject.transform.GetChild(i).gameObject);
                    }
                }
            }

            targetWaypoint = GetClosestPointOnPath(transform.position);
        }
    }

    void Update()
    {
        elapsedTime = Time.time - startTime;
        if (endWalkTime == GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.Hour)
        {
            path.Clear();
            late = true;
            startPath = true;
            isStopped = false;
            Unhide();
            SetPointsByChildren(false);
        }
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

                        currentWaypointIndex++;
                        hadLoiter = false;
                      
                        if (currentWaypointIndex >= path.Count)
                        {
                            if(late)
                            {
                                first = true;
                                path.Clear();
                                id = Random.Range(1, 8);
                                SetPointsByChildren(first);
                                gameObject.SetActive(false);
                            }
                            else if(first)
                            {
                                first = false;
                                path.Clear();
                                SetPointsByChildren(first);
                            }
                            else
                            {
                                path.Clear();
                                id = Random.Range(1, 8);
                                SetPointsByChildren(first);
                            }
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
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    private IEnumerator LoiterInside(int time)
    {
        yield return new WaitForSeconds(time);
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        inside = false;
        Unhide();
    }

    public IEnumerator Talk(int time, Vector3 pos)
    {
        rb.velocity = Vector3.zero;
        animator.runtimeAnimatorController = Resources.Load("BasicMotions@Talk") as RuntimeAnimatorController;
        isStopped = true;
        Quaternion rotation = Quaternion.LookRotation(pos);
        transform.localRotation = new Quaternion(transform.localRotation.x, rotation.y, transform.localRotation.z,transform.localRotation.w);
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        yield return new WaitForSeconds(time);
        isStopped = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        animator.runtimeAnimatorController = Resources.Load("BasicMotions@Walk") as RuntimeAnimatorController;
    }
}
