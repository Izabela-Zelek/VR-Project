using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the spawning of new vehicles and assigning road paths
/// </summary>
public class VehicleController : MonoBehaviour
{
    private int _roadChoice;
    private int _nextSpawn;
    private Object[] _carTypes;
    private GameObject _chosenCar;
    private GameObject _newCar;
    
    /// <summary>
    /// Generates a random time interval before next vehicle spawn, generates a random id of a roadpath to assign
    /// Generates a random car model from a list of all car models
    /// </summary>
    private void Start()
    {
        _carTypes = Resources.LoadAll("Vehicles", typeof(GameObject));
        _nextSpawn = Random.Range(7, 15);
        _roadChoice = Random.Range(0, 12);

        if(_roadChoice == 6 || _roadChoice == 7 || _roadChoice == 8)
        {
            while ((_roadChoice == 6 || _roadChoice == 7 || _roadChoice == 8) && transform.GetChild(2).GetChild(_roadChoice).transform.Find("Parking1").GetComponent<RoadCellController>().HasCar && transform.GetChild(2).GetChild(_roadChoice).transform.Find("Parking2").GetComponent<RoadCellController>().HasCar)
            {
                _roadChoice = Random.Range(0, 12);
            }
        }
        else if (_roadChoice == 9 || _roadChoice == 10 || _roadChoice == 11)
        {
            while ((_roadChoice == 9 || _roadChoice == 10 || _roadChoice == 11) && transform.GetChild(2).GetChild(_roadChoice).transform.Find("Parking1").GetComponent<RoadCellController>().HasCar && transform.GetChild(2).GetChild(_roadChoice).transform.Find("Parking2").GetComponent<RoadCellController>().HasCar)
            {
                 _roadChoice = Random.Range(0, 12);
            }
        }
        _chosenCar = (GameObject)_carTypes[Random.Range(6, _carTypes.Length)];

        StartCoroutine(WaitForSpawn());
    }

    /// <summary>
    /// Spawns a vehicle each time the wait interval is over and assigns them their roadpath
    /// Generates a random time interval before next vehicle spawn for next car, generates a random id of a roadpath to assign for next car
    /// Generates a random car model from a list of all car models for next car
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForSpawn()
    {
        while (true)
        {
            Vector3 startPos = new Vector3();
            Quaternion rot = new Quaternion();
            if(_roadChoice == 0 || _roadChoice == 1 || _roadChoice == 6 || _roadChoice == 9)
            {
                startPos = GameObject.Find("Entr1").transform.position;
                rot = GameObject.Find("Entr1").transform.rotation;
            }
            else if(_roadChoice == 2 || _roadChoice == 3 || _roadChoice == 7 || _roadChoice == 10)
            {
                startPos = GameObject.Find("Entr2").transform.position;
                rot = GameObject.Find("Entr2").transform.rotation;
            }
            else
            {
                startPos = GameObject.Find("Entr3").transform.position;
                rot = GameObject.Find("Entr3").transform.rotation;
            }
            _newCar = Instantiate(_chosenCar,new Vector3(startPos.x, startPos.y + _chosenCar.transform.position.y,startPos.z), rot, this.transform.GetChild(3).transform) as GameObject;
            _newCar.GetComponent<VehicleMover>().SetPointsByChildren(transform.GetChild(2).GetChild(_roadChoice).gameObject);
            _nextSpawn = Random.Range(3, 11);
            _roadChoice = Random.Range(0, 12);

            if (_roadChoice == 6 || _roadChoice == 7 || _roadChoice == 8)
            {
                while ((_roadChoice == 6 || _roadChoice == 7 || _roadChoice == 8) && transform.GetChild(2).GetChild(_roadChoice).transform.Find("Parking1").GetComponent<RoadCellController>().HasCar && transform.GetChild(2).GetChild(_roadChoice).transform.Find("Parking2").GetComponent<RoadCellController>().HasCar)
                {
                     _roadChoice = Random.Range(0, 12);
                }
            }
            else if (_roadChoice == 9 || _roadChoice == 10 || _roadChoice == 11)
            {
                while ((_roadChoice == 9 || _roadChoice == 10 || _roadChoice == 11) && transform.GetChild(2).GetChild(_roadChoice).transform.Find("Parking1").GetComponent<RoadCellController>().HasCar && transform.GetChild(2).GetChild(_roadChoice).transform.Find("Parking2").GetComponent<RoadCellController>().HasCar)
                {
                    _roadChoice = Random.Range(0, 12);
                }
            }
            _chosenCar = (GameObject)_carTypes[Random.Range(6, _carTypes.Length)];

            yield return new WaitForSeconds(_nextSpawn);
        }
    }
}
