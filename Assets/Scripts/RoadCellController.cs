using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the cells in each possible vehicle path. Specifies whether the cell is at a traffic light or is a parking space. Keeps track of all paths that share the common cell.
/// </summary>
public class RoadCellController : MonoBehaviour
{
    public bool InFrontOfLight;
    public bool IsParking;
    public bool HasCar;
    public int TrafficLightInFront;

    [SerializeField]
    private int _neighbour1Nr;
    [SerializeField]
    private int _neighbour2Nr;
    [SerializeField]
    private int _parkingNr;

    /// <summary>
    /// If it is a parking space, gets the id of the space based on the name of the object
    /// </summary>
    private void Start()
    {
        if(IsParking)
        {
            string[] nameSplit = name.Split(new string[] { "Parking" }, System.StringSplitOptions.None);
            _parkingNr = int.Parse(nameSplit[1]);
        }
    }

    public void SetParked(bool parked)
    {
        HasCar = parked;
    }
    public int GetNeighbour1()
    {
        return _neighbour1Nr;
    }
    public int GetNeighbour2()
    {
        return _neighbour2Nr;
    }

    public int GetParkingNr()
    {
        return _parkingNr;
    }
}
