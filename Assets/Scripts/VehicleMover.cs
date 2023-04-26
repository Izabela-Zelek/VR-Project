using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Handles the path following steering behaviour of vehicles
/// </summary>
public class VehicleMover : MonoBehaviour
{
    public List<GameObject> Path;
    public float Speed = 5.0f;
    public float Mass = 5.0f;
    public float MaxSteer = 15.0f;
    public float PathRadius = 1.0f;
    public int CurrentWaypointIndex = 0;
    public int Id = -1;

    private Vector3 _targetWaypoint;
    private Vector3 _desiredVelocity;
    private Vector3 _steeringForce;
    private Rigidbody _rb;
    private float _yPos;
    private bool _pathForward = true;
    private GameObject _traffic;
    private GameObject _parkingMan;
    private GameObject _parkingManObject;
    private bool _canMove = true;
    private bool _stoppedAtLight = false;
    private bool _parked = false;
    private AudioSource _drive;

    private void Start()
    {
        _drive = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody>();
        _yPos = transform.localPosition.y;
        _traffic = GameObject.Find("Traffic").gameObject;
        _parkingMan = Resources.Load("DudeParking") as GameObject;
    }

    /// <summary>
    /// Iterates through each child of the passed in gameobject and adds their positions to a list
    /// Checks whether a position cell is a parking space, ends iteration if it is true
    /// Calls for function to find closest position in the list
    /// </summary>
    /// <param name="parent"></param>
    public void SetPointsByChildren(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if(!parent.transform.GetChild(i).GetComponent<RoadCellController>().HasCar)
            {
                Path.Add(parent.transform.GetChild(i).gameObject);

                if(parent.transform.GetChild(i).GetComponent<RoadCellController>().IsParking)
                {
                    parent.transform.GetChild(i).GetComponent<RoadCellController>().SetParked(true);
                    int neighbour1 = parent.transform.GetChild(i).GetComponent<RoadCellController>().GetNeighbour1();
                    int neighbour2 = parent.transform.GetChild(i).GetComponent<RoadCellController>().GetNeighbour2();
                    int parkingNr = parent.transform.GetChild(i).GetComponent<RoadCellController>().GetParkingNr();

                    parent.transform.parent.transform.GetChild(neighbour1).Find("Parking" + parkingNr).GetComponent<RoadCellController>().SetParked(true);
                    parent.transform.parent.transform.GetChild(neighbour2).Find("Parking" + parkingNr).GetComponent<RoadCellController>().SetParked(true);
                    break;
                }
            }
        }

        _targetWaypoint = GetClosestPointOnPath(transform.position);
    }
    /// <summary>
    /// Starting from the previously calculated closest position, once the vehicle position is within bounds of the next position in the list, they current waypoint gets incremented
    /// Destroys gameobject once it reaches the end and isn't parked
    /// Checks the current position cell for whether it is at a traffic light
    /// If so, checks if that traffic light is red, if it is, vehicle stops
    /// Adds force to gameobject to move in the direction of the next point - with smoothing
    /// </summary>
    private void Update()
    {
        if (Path.Count > 0 && _canMove && !_parked)
        {
            if(!_drive.isPlaying)
            {
                _drive.Play();
            }

            float distance = Vector3.Distance(transform.position, _targetWaypoint);
            if (!_stoppedAtLight)
            {
                _rb.constraints = RigidbodyConstraints.None;
                _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
            if (distance <= PathRadius)
            {
                if (Path[CurrentWaypointIndex].GetComponent<RoadCellController>().InFrontOfLight)
                {
                    _stoppedAtLight = true;
                    int g2 = _traffic.GetComponent<TrafficLightController>().ReturnGreen2();
                    if (Path[CurrentWaypointIndex].GetComponent<RoadCellController>().TrafficLightInFront == _traffic.GetComponent<TrafficLightController>().ReturnGreen1())
                    {
                        _stoppedAtLight = false;
                    }
                    else if(Path[CurrentWaypointIndex].GetComponent<RoadCellController>().TrafficLightInFront == _traffic.GetComponent<TrafficLightController>().ReturnGreen2())
                    {
                        _stoppedAtLight = false;
                    }
                }
                if (!_stoppedAtLight)
                {
                    if (_rb.constraints == RigidbodyConstraints.FreezePositionX || _rb.constraints == RigidbodyConstraints.FreezePositionY || _rb.constraints == RigidbodyConstraints.FreezePositionZ || _rb.constraints == RigidbodyConstraints.FreezeRotationY)
                    {
                        _rb.constraints = RigidbodyConstraints.None;
                        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                    }

                    if (!_parked)
                    {
                        if (CurrentWaypointIndex >= Path.Count - 1 && _pathForward)
                        {
                            if (!Path[CurrentWaypointIndex].GetComponent<RoadCellController>().IsParking)
                            {
                                Destroy(this.gameObject);
                            }
                            else
                            {
                                _parked = true;
                                _parkingManObject = Instantiate(_parkingMan, transform.GetChild(5).transform.position, Quaternion.identity, transform);
                            }
                        }
                        else
                        {
                            CurrentWaypointIndex++;
                            _targetWaypoint = Path[CurrentWaypointIndex].transform.position;
                        }
                    }
                }
                else
                {
                    _rb.velocity = Vector3.zero;
                    _rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                }
            }

            _desiredVelocity = (_targetWaypoint - transform.position).normalized * Speed;
            _steeringForce = _desiredVelocity - _rb.velocity;
            _steeringForce /= Mass;

            if (_steeringForce.magnitude > MaxSteer)
            {
                _steeringForce = _steeringForce.normalized * MaxSteer;
            }

            _rb.AddForce(_steeringForce);
            Vector3 vel = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

            if(vel != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(vel);
                transform.rotation *= Quaternion.Euler(0f, -90f, 0f);
                transform.localPosition = new Vector3(transform.localPosition.x, _yPos, transform.localPosition.z);
            }
        }
        else
        {
            if (_drive.isPlaying)
            {
                _drive.Stop();
            }
            transform.localPosition = new Vector3(transform.localPosition.x, _yPos, transform.localPosition.z);
            _rb.velocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
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

    public void SetMove(bool move)
    {
        _canMove = move;
    }
}