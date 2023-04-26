using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Handles the path following steering behaviour of vehicles
/// </summary>
public class VehicleMover : MonoBehaviour
{
    public List<GameObject> path;
    public float speed = 5.0f;
    public float mass = 5.0f;
    public float maxSteer = 15.0f;
    public float pathRadius = 1.0f;

    public int currentWaypointIndex = 0;
    private Vector3 targetWaypoint;
    private Vector3 desiredVelocity;
    private Vector3 steeringForce;
    private Rigidbody rb;
    private float yPos;
    private bool pathForward = true;
    public int id = -1;

    private GameObject traffic;
    private GameObject parkingMan;
    private GameObject parkingManObject;
    private bool canMove = true;
    private bool stoppedAtLight = false;
    private bool parked = false;
    private AudioSource _drive;
    void Start()
    {
        _drive = GetComponent<AudioSource>();
        if (id == -1)
        {
            id = Random.Range(0, 10000);
        }
        rb = GetComponent<Rigidbody>();
        yPos = transform.localPosition.y;
        traffic = GameObject.Find("Traffic").gameObject;
        parkingMan = Resources.Load("DudeParking") as GameObject;
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
                path.Add(parent.transform.GetChild(i).gameObject);

                if(parent.transform.GetChild(i).GetComponent<RoadCellController>().IsParking)
                {
                    parent.transform.GetChild(i).GetComponent<RoadCellController>().setParked(true);
                    int neighbour1 = parent.transform.GetChild(i).GetComponent<RoadCellController>().GetNeighbour1();
                    int neighbour2 = parent.transform.GetChild(i).GetComponent<RoadCellController>().GetNeighbour2();
                    int parkingNr = parent.transform.GetChild(i).GetComponent<RoadCellController>().GetParkingNr();

                    parent.transform.parent.transform.GetChild(neighbour1).Find("Parking" + parkingNr).GetComponent<RoadCellController>().setParked(true);
                    parent.transform.parent.transform.GetChild(neighbour2).Find("Parking" + parkingNr).GetComponent<RoadCellController>().setParked(true);
                    break;
                }
            }
        }

        targetWaypoint = GetClosestPointOnPath(transform.position);
    }
    /// <summary>
    /// Starting from the previously calculated closest position, once the vehicle position is within bounds of the next position in the list, they current waypoint gets incremented
    /// Destroys gameobject once it reaches the end and isn't parked
    /// Checks the current position cell for whether it is at a traffic light
    /// If so, checks if that traffic light is red, if it is, vehicle stops
    /// Adds force to gameobject to move in the direction of the next point - with smoothing
    /// </summary>
    void Update()
    {
        if (path.Count > 0 && canMove && !parked)
        {
            if(!_drive.isPlaying)
            {
                _drive.Play();
            }

            float distance = Vector3.Distance(transform.position, targetWaypoint);
            if (!stoppedAtLight)
            {
                rb.constraints = RigidbodyConstraints.None;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
            if (distance <= pathRadius)
            {
                if (path[currentWaypointIndex].GetComponent<RoadCellController>().InFrontOfLight)
                {
                    stoppedAtLight = true;
                    int g2 = traffic.GetComponent<TrafficLightController>().ReturnGreen2();
                    if (path[currentWaypointIndex].GetComponent<RoadCellController>().TrafficLightInFront == traffic.GetComponent<TrafficLightController>().ReturnGreen1())
                    {
                        stoppedAtLight = false;
                    }
                    else if(path[currentWaypointIndex].GetComponent<RoadCellController>().TrafficLightInFront == traffic.GetComponent<TrafficLightController>().ReturnGreen2())
                    {
                        stoppedAtLight = false;
                    }
                }
                if (!stoppedAtLight)
                {
                    if (rb.constraints == RigidbodyConstraints.FreezePositionX || rb.constraints == RigidbodyConstraints.FreezePositionY || rb.constraints == RigidbodyConstraints.FreezePositionZ || rb.constraints == RigidbodyConstraints.FreezeRotationY)
                    {
                        rb.constraints = RigidbodyConstraints.None;
                        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                    }

                    if (!parked)
                    {
                        if (currentWaypointIndex >= path.Count - 1 && pathForward)
                        {
                            if (!path[currentWaypointIndex].GetComponent<RoadCellController>().IsParking)
                            {
                                Destroy(this.gameObject);
                            }
                            else
                            {
                                parked = true;
                                parkingManObject = Instantiate(parkingMan, transform.GetChild(5).transform.position, Quaternion.identity, transform);
                            }
                        }
                        else
                        {
                            currentWaypointIndex++;
                            targetWaypoint = path[currentWaypointIndex].transform.position;
                        }
                    }
                }
                else
                {
                    rb.velocity = Vector3.zero;
                    rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                }
            }

            desiredVelocity = (targetWaypoint - transform.position).normalized * speed;
            steeringForce = desiredVelocity - rb.velocity;
            steeringForce /= mass;

            if (steeringForce.magnitude > maxSteer)
            {
                steeringForce = steeringForce.normalized * maxSteer;
            }

            rb.AddForce(steeringForce);
            Vector3 vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            if(vel != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(vel);
                transform.rotation *= Quaternion.Euler(0f, -90f, 0f);
                transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);
            }
        }
        else
        {
            if (_drive.isPlaying)
            {
                _drive.Stop();
            }
            transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
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

    public void setMove(bool move)
    {
        canMove = move;
    }
}