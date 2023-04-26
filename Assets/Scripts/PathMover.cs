using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Handles the path following steering behaviour of civilian NPCs
/// </summary>
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
    /// <summary>
    /// Calls for function to set up initial path - initially a path leading NPC out of NPC estate
    /// Generates a random wake up and sleep time
    /// </summary>
    void Start()
    {
        if (id == -1)
        {
            GetClosestPath(transform.position);
        }
        rb = GetComponent<Rigidbody>();
        SetPointsByChildren(first);
        yPos = transform.localPosition.y;
        animator = GetComponent<Animator>();
        startWalkTime = Random.Range(7, 13);
        endWalkTime = Random.Range(17, 21);
        startTime = Time.time;
    }

    /// <summary>
    /// Sets up the first default path of the NPC by searching for the path gameobject with passed in newId
    /// If the id is 1, it adds the path twice, once going forward, once going back
    /// Calls for function which finds closest position of the path
    /// </summary>
    /// <param name="newId"></param>
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

    /// <summary>
    /// If current world time is equal to the NPC's sleep time, finds path which will lead NPC home
    /// If this is the first path the NPC is taking that current day, finds the path which will lead the NPC out of the NPC estate
    /// If neither of those apply, finds the path with the assigned path id
    /// Calls for function to find closest point of assigned path
    /// </summary>
    /// <param name="first"></param>
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

    /// <summary>
    /// If time in world is equal to sleeping time, clears current path and calls for function to find path home, also enables the NPC model if it was hidden
    /// If the NPC is outside and they have a path, they will start walking that path
    /// Starting from the previously calculated closest position, once the NPC position is within bounds of the next position in the list, they current waypoint gets incremented
    /// Checks current position cell for loiter value, starts loiter coroutine if greater than 0
    /// Checks current position cell for bool determining if its inside a shop, hides NPC if so
    /// Changes animation based on whether they are walking or loitering/inside
    /// Assigns a new random path once NPC reaches the end of current one
    /// Adds force to gameobject to move in the direction of the next point - with smoothing
    /// </summary>
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
                    transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);

                }
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

    private Vector3 GetClosestPath(Vector3 position)
    {
        Vector3 closestPoint = Vector3.zero;
        int closestPath = -1;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < 8; i++)
        {
            GameObject pathObject = GameObject.Find("Path" + i);
            List<GameObject> tempPath = new List<GameObject>();
            for (int j = 0; j < pathObject.transform.childCount; j++)
            {
                path.Add(pathObject.transform.GetChild(i).gameObject);
            }
            for (int j = 0; j < path.Count; j++)
            {
                Vector3 pathPoint = path[j].transform.position;
                float distance = Vector3.Distance(position, pathPoint);
                if (distance < closestDistance)
                {
                    closestPath = i;
                    closestDistance = distance;
                    closestPoint = pathPoint;
                    currentWaypointIndex = j;
                }
            }
        }

        id = closestPath;
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
    /// <summary>
    /// Disables the NPC model when entering a shop
    /// </summary>
    private void Hide()
    {
        for(int i = 0; i < transform.childCount; i++)
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
        rb.constraints = RigidbodyConstraints.FreezePositionY;
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
    /// <summary>
    /// Coroutine which allows the NPC to stop for a chat with an NPC they bump into
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
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
