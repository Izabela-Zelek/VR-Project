using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    private int _roadChoice;
    private int _nextSpawn;
    private Object[] _carTypes;
    private GameObject _chosenCar;
    private GameObject _newCar;
    // Start is called before the first frame update
    void Start()
    {
        _carTypes = Resources.LoadAll("Vehicles", typeof(GameObject));
        _nextSpawn = Random.Range(3, 11);
        _roadChoice = Random.Range(0, 6);
        _chosenCar = (GameObject)_carTypes[Random.Range(6, _carTypes.Length)];

        StartCoroutine(WaitForSpawn());
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);

    }
    private IEnumerator WaitForSpawn()
    {
        while (true)
        {
            Vector3 startPos = new Vector3();
            Quaternion rot = new Quaternion();
            if(_roadChoice == 0 || _roadChoice == 1)
            {
                startPos = GameObject.Find("Entr1").transform.position;
                rot = GameObject.Find("Entr1").transform.rotation;
            }
            else if(_roadChoice == 2 || _roadChoice == 3)
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
            // _nextSpawn = Random.Range(3, 11);
            _nextSpawn = 10;
            _roadChoice = Random.Range(0, 6);
            _chosenCar = (GameObject)_carTypes[Random.Range(6, _carTypes.Length)];

            yield return new WaitForSeconds(_nextSpawn);
        }
    }
}
