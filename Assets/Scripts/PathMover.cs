using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Handles the path following steering behaviour of civilian NPCs
/// </summary>
public class PathMover : MonoBehaviour
{
    public List<GameObject> Path;
    public float Speed = 5.0f;
    public float Mass = 5.0f;
    public float MaxSteer = 15.0f;
    public float PathRadius = 1.0f;
    public int CurrentWaypointIndex = 0;
    public int Id = -1;
    public bool HadLoiter = false;
    public bool CanChangeY = true;
    public int StartWalkTime = 8;
    public int EndWalkTime = 17;
    public bool First = false;
    public bool Late = false;
    public bool Inside = false;

    private Animator _animator;
    private bool _isStopped = false;
    private Vector3 _targetWaypoint;
    private Vector3 _desiredVelocity;
    private Vector3 _steeringForce;
    private Rigidbody _rb;
    private float _yPos;
    private bool _onGround = true;
    private float _startTime;
    private float _duration = 10.0f;
    private float _elapsedTime;
    private bool _startPath = false;
    
    /// <summary>
    /// Calls for function to set up initial path - initially a path leading NPC out of NPC estate
    /// Generates a random wake up and sleep time
    /// </summary>
    private void Start()
    {
        if (Id == -1)
        {
            Id = Random.Range(10, 10000);
        }
        _rb = GetComponent<Rigidbody>();
        SetPointsByChildren(First);
        _yPos = transform.localPosition.y;
        _animator = GetComponent<Animator>();
        StartWalkTime = Random.Range(7, 13);
        EndWalkTime = Random.Range(17, 21);
        _startTime = Time.time;
    }

    /// <summary>
    /// Sets up the first default path of the NPC by searching for the path gameobject with passed in newId
    /// If the id is 1, it adds the path twice, once going forward, once going back
    /// Calls for function which finds closest position of the path
    /// </summary>
    /// <param name="newId"></param>
    public void SetDefaultPath(int newId)
    {
        Path.Clear();
        Id = newId;
        GameObject pathObject = GameObject.Find("Path" + Id);
        for (int i = 0; i < pathObject.transform.childCount; i++)
        {
            Path.Add(pathObject.transform.GetChild(i).gameObject);
        }

        if (Id == 1)
        {
            for (int i = pathObject.transform.childCount - 1; i > 0; i--)
            {
                Path.Add(pathObject.transform.GetChild(i).gameObject);
            }
        }

        _targetWaypoint = GetClosestPointOnPath(transform.position);
        transform.LookAt(new Vector3(_targetWaypoint.x, 0, _targetWaypoint.z));
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
        if (Id != -1 && Path.Count == 0)
        {
            if (Late)
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
                                Path.Add(pathObject.transform.GetChild(i).gameObject);
                            }
                            else if (pathObject.transform.GetChild(i).name.Contains("MeetingPoint"))
                            {
                                Path.Add(pathObject.transform.GetChild(i).gameObject);
                            }
                            else if (pathObject.transform.GetChild(i).name == npcNr.ToString())
                            {
                                Path.Add(pathObject.transform.GetChild(i).gameObject);
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
                            Path.Add(pathObject.transform.GetChild(i).gameObject);
                        }
                        else if (pathObject.transform.GetChild(i).name.Contains("MeetingPoint"))
                        {
                            Path.Add(pathObject.transform.GetChild(i).gameObject);
                        }
                        else if (pathObject.transform.GetChild(i).name == npcNr.ToString())
                        {
                            Path.Add(pathObject.transform.GetChild(i).gameObject);
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
                    Path.Add(pathObject.transform.GetChild(i).gameObject);
                }

            }
            else
            {
                GameObject pathObject = GameObject.Find("Path" + Id);
                for (int i = 0; i < pathObject.transform.childCount; i++)
                {
                    Path.Add(pathObject.transform.GetChild(i).gameObject);
                }

                if (Id == 1)
                {
                    for (int i = pathObject.transform.childCount - 1; i > 0; i--)
                    {
                        Path.Add(pathObject.transform.GetChild(i).gameObject);
                    }
                }
            }

            _targetWaypoint = GetClosestPointOnPath(transform.position);
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
    private void Update()
    {
        _elapsedTime = Time.time - _startTime;
        if (EndWalkTime == GameObject.Find("GameManager").GetComponent<TimeController>().CurrentTime.Hour)
        {
            Path.Clear();
            Late = true;
            _startPath = true;
            _isStopped = false;
            Unhide();
            SetPointsByChildren(false);
        }
        if (Path.Count > 0 && !Inside)
        {
            if (!_startPath)
            {
                if (StartWalkTime == GameObject.Find("GameManager").GetComponent<TimeController>().CurrentTime.Hour)
                {
                    _startPath = true;
                    if (_animator.runtimeAnimatorController.name != "BasicMotions@Walk")
                    {
                        _animator.runtimeAnimatorController = Resources.Load("BasicMotions@Walk") as RuntimeAnimatorController;
                    }
                }
            }
            if (_startPath)
            {
                float distance = Vector3.Distance(transform.position, _targetWaypoint);

                if (distance <= PathRadius)
                {
                    if (Path[CurrentWaypointIndex].GetComponent<PathCellController>().GetLoiterTime() > 0 && !HadLoiter)
                    {
                        _rb.velocity = Vector3.zero;
                        _animator.runtimeAnimatorController = Resources.Load("BasicMotions@Talk") as RuntimeAnimatorController;
                        _isStopped = true;
                        StartCoroutine(Loiter(Path[CurrentWaypointIndex].GetComponent<PathCellController>().GetLoiterTime()));
                    }

                    if(Path[CurrentWaypointIndex].GetComponent<PathCellController>().GetAtShop())
                    {
                        Hide();
                        int hideFor = Random.Range(9, 20);
                        StartCoroutine(LoiterInside(hideFor));
                    }
                    if (!_isStopped && !Inside)
                    {
                        if (_animator.runtimeAnimatorController.name != "BasicMotions@Walk")
                        {
                            _animator.runtimeAnimatorController = Resources.Load("BasicMotions@Walk") as RuntimeAnimatorController;
                            HadLoiter = false;
                        }

                        CurrentWaypointIndex++;
                        HadLoiter = false;
                      
                        if (CurrentWaypointIndex >= Path.Count)
                        {
                            if(Late)
                            {
                                First = true;
                                Path.Clear();
                                Id = Random.Range(1, 8);
                                SetPointsByChildren(First);
                                gameObject.SetActive(false);
                            }
                            else if(First)
                            {
                                First = false;
                                Path.Clear();
                                SetPointsByChildren(First);
                            }
                            else
                            {
                                Path.Clear();
                                Id = Random.Range(1, 8);
                                SetPointsByChildren(First);
                            }
                        }
                        _targetWaypoint = Path[CurrentWaypointIndex].transform.position;
                    }

                }

                if (!_isStopped)
                {
                    _desiredVelocity = (_targetWaypoint - transform.position).normalized * Speed;
                    _steeringForce = _desiredVelocity - _rb.velocity;
                    _steeringForce /= Mass;

                    if (_steeringForce.magnitude > MaxSteer)
                    {
                        _steeringForce = _steeringForce.normalized * MaxSteer;
                    }

                    _rb.AddForce(_steeringForce);
                    if (_rb.velocity != Vector3.zero)
                    {
                        Quaternion lookRotation = Quaternion.LookRotation(_rb.velocity, Vector3.up);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
                    }
                    transform.localPosition = new Vector3(transform.localPosition.x, _yPos, transform.localPosition.z);

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

        for (int i = 0; i < Path.Count; i++)
        {
            Vector3 pathPoint = Path[i].transform.position;
            float distance = Vector3.Distance(position, pathPoint);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = pathPoint;
                CurrentWaypointIndex = i;
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
        _isStopped = false;
        HadLoiter = true;
    }

    /// <summary>
    /// Checks for collisions with curb objects to boost the NPC up or down curbs
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (_elapsedTime >= _duration)
        {
            _startTime = Time.time;
            CanChangeY = true;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Curb") && CanChangeY)
        {
            if(_onGround)
            {
                _yPos = transform.localPosition.y + 0.05f;
                _onGround = false;
            }
           else
            {
                _yPos = transform.localPosition.y - 0.05f;
                _onGround = false;
            }
            CanChangeY = false;
        }
    }


    public int GetStartTime()
    {
        return StartWalkTime;
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
        _rb.velocity = Vector3.zero;
        _rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

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
        _rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    /// <summary>
    /// Coroutine which allows the NPC to loiter at a shop for an passed in amount of time
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator LoiterInside(int time)
    {
        yield return new WaitForSeconds(time);
        _rb.constraints = RigidbodyConstraints.FreezePositionY;
        Inside = false;
        Unhide();
    }

    /// <summary>
    /// Coroutine which allows the NPC to stop for a chat with an NPC they bump into
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator Talk(int time, Vector3 pos)
    {
        _rb.velocity = Vector3.zero;
        _animator.runtimeAnimatorController = Resources.Load("BasicMotions@Talk") as RuntimeAnimatorController;
        _isStopped = true;
        Quaternion rotation = Quaternion.LookRotation(pos);
        transform.localRotation = new Quaternion(transform.localRotation.x, rotation.y, transform.localRotation.z,transform.localRotation.w);
        _rb.velocity = Vector3.zero;
        _rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        yield return new WaitForSeconds(time);
        _isStopped = false;
        _rb.constraints = RigidbodyConstraints.FreezePositionY;
        _animator.runtimeAnimatorController = Resources.Load("BasicMotions@Walk") as RuntimeAnimatorController;
    }
}
