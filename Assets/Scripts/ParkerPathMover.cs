using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Handles the path following steering behaviour of visiting NPCs
/// </summary>
public class ParkerPathMover : MonoBehaviour
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
    public bool First = false;
    public bool Inside = false;

    private Animator _animator;
    private bool _isStopped = false;
    private Vector3 _targetWaypoint;
    private Vector3 _desiredVelocity;
    private Vector3 _steeringForce;
    private Rigidbody _rb;
    private float _yPos;
    private bool _pathForward = true;
    private bool _onGround = true;
    private float _startTime;
    private float _duration = 10.0f;
    private float _elapsedTime;

    /// <summary>
    /// Calls for function to set up initial path
    /// </summary>
    private void Start()
    {
        if (Id == -1)
        {
            Id = Random.Range(1, 8);
        }
        _rb = GetComponent<Rigidbody>();
        SetPointsByChildren(First);
        _yPos = 0.1f;
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Finds the path gameobject with the assigned path id and adds all children positions to a list
    /// Calls for function to find the closest position in that list
    /// </summary>
    /// <param name="first"></param>
    private void SetPointsByChildren(bool first)
    {
        if (Id != -1 && Path.Count == 0)
        {
            if(first)
            {
                GameObject pathObject = GameObject.Find("Path" + 0.1);
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

            }

            _targetWaypoint = GetClosestPointOnPath(transform.position);
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
        _elapsedTime = Time.time - _startTime;
        if (Path.Count > 0 && !Inside)
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
                    if (_pathForward)
                    {
                        CurrentWaypointIndex++;
                        HadLoiter = false;
                    }
                    else if (!_pathForward)
                    {
                        CurrentWaypointIndex--;
                        HadLoiter = false;
                    }
                    if (CurrentWaypointIndex >= Path.Count - 1 && _pathForward)
                    {
                        if(First)
                        {
                            First = false;
                            Path.Clear();
                            Id = Random.Range(1, 8);
                            SetPointsByChildren(First);
                        }
                        else
                        {
                            _pathForward = false;
                            CurrentWaypointIndex = Path.Count - 1;
                        }
                    }
                    else if (CurrentWaypointIndex < 0 && !_pathForward)
                    {
                        _pathForward = true;
                        CurrentWaypointIndex = 0;
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
                transform.position = new Vector3(transform.position.x, _yPos, transform.position.z);

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
}
