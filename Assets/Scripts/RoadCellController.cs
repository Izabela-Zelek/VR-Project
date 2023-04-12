using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCellController : MonoBehaviour
{
    public bool InFrontOfLight;
    public bool IsParking;
    public bool HasCar;
    public int TrafficLightInFront;
    [SerializeField]
    private int Neighbour1Nr;
    [SerializeField]
    private int Neighbour2Nr;
    [SerializeField]
    private int parkingNr;

    private void Start()
    {
        if(IsParking)
        {
            string[] nameSplit = name.Split(new string[] { "Parking" }, System.StringSplitOptions.None);
            parkingNr = int.Parse(nameSplit[1]);
        }
    }

    public void setParked(bool parked)
    {
        HasCar = parked;
    }
    public int GetNeighbour1()
    {
        return Neighbour1Nr;
    }
    public int GetNeighbour2()
    {
        return Neighbour2Nr;
    }

    public int GetParkingNr()
    {
        return parkingNr;
    }
}
