using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Handles the path following steering behaviour of shopkeeper NPCs
/// </summary>
public class ShopKeeperMover : MonoBehaviour
{
    public List<GameObject> Path;
    public float Speed = 5.0f;
    public float Mass = 5.0f;
    public float MaxSteer = 15.0f;
    public float PathRadius = 0.001f;
    public int CurrentWaypointIndex = 0;
    public int Id = -1;
    public bool CanChangeY = true;
    public int StartWalkTime = 4;
    public int ShopRotation = 0;
    public int DayOff = 0;

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
    private bool _startPath = false;
    private int _endWalkTime = 17;
    private bool _working = false;
    
    /// <summary>
    /// Calls for function to set up initial path
    /// </summary>
    private void Start()
    { 
        _rb = GetComponent<Rigidbody>();
        SetPointsByChildren();
        _yPos = transform.localPosition.y;
        _animator = GetComponent<Animator>();
        _startTime = Time.time;
    }
    /// <summary>
    /// Finds the path gameobject with the assigned path id and adds all children positions to a list
    /// Calls for function to find the closest position in that list
    /// Increments path by one as the first cell is the house doorstep
    /// </summary>
    private void SetPointsByChildren()
    {
        if (Id != -1 && Path.Count == 0)
        {
            GameObject pathObject = GameObject.Find("Path" + Id);
            for (int i = 0; i < pathObject.transform.childCount; i++)
            {
                Path.Add(pathObject.transform.GetChild(i).gameObject);
            }

            _targetWaypoint = GetClosestPointOnPath(transform.position);
            CurrentWaypointIndex++;
            _targetWaypoint = Path[CurrentWaypointIndex].transform.position;
        }
    }
    /// <summary>
    /// If the NPC is outside and they have a path, they will start walking that path
    /// Starting from the previously calculated closest position, once the NPC position is within bounds of the next position in the list, they current waypoint gets incremented
    /// Changes animation based on whether they are walking or attending their shop
    /// Iterates backwards through path once the current world time is equal to their sleep time
    /// Adds force to gameobject to move in the direction of the next point - with smoothing
    /// </summary>
    private void Update()
    {
        _elapsedTime = Time.time - _startTime;
        if (Path.Count > 0 && !_working)
        {
            if(!_startPath)
            {
                if (StartWalkTime == GameObject.Find("GameManager").GetComponent<TimeController>().CurrentTime.Hour && WorkToday())
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
                    if (!_isStopped)
                    {
                        if (_animator.runtimeAnimatorController.name != "BasicMotions@Walk")
                        {
                            _animator.runtimeAnimatorController = Resources.Load("BasicMotions@Walk") as RuntimeAnimatorController;
                        }
                        if (CurrentWaypointIndex >= Path.Count - 1 && _pathForward)
                        {
                            _working = true;
                            _pathForward = false;
                            CurrentWaypointIndex = Path.Count - 1;
                        }
                        if (_pathForward && !_working)
                        {
                            CurrentWaypointIndex++;
                        }
                        else if (!_pathForward && !_working)
                        {
                            CurrentWaypointIndex--;
                        }
                        
                       if (CurrentWaypointIndex < 0 && !_pathForward && !_working)
                        {
                            _pathForward = true;
                            CurrentWaypointIndex = 1;
                            gameObject.SetActive(false);
                        }
                        if (!_working)
                        { 
                            _targetWaypoint = Path[CurrentWaypointIndex].transform.position; 
                        }
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
        else if(_working)
        {
            _animator.runtimeAnimatorController = Resources.Load("BasicMotions@Idle") as RuntimeAnimatorController;
            transform.rotation = Quaternion.Euler(0f, ShopRotation, 0f);
            _rb.velocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }

        if (_endWalkTime == GameObject.Find("GameManager").GetComponent<TimeController>().CurrentTime.Hour)
        {
            _working = false;
            _startPath = true;
            if (_animator.runtimeAnimatorController.name != "BasicMotions@Walk")
            {
                _animator.runtimeAnimatorController = Resources.Load("BasicMotions@Walk") as RuntimeAnimatorController;
            }
            _rb.constraints = RigidbodyConstraints.FreezePositionY;
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
    /// Returns whether or not the NPC is supposed to be working on the current day
    /// </summary>
    /// <returns></returns>
    public bool WorkToday()
    {
        return GameObject.Find("GameManager").GetComponent<TimeController>().GetDayOfWeek() != DayOff;
    }
}
