using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    private int _trafficWait = 10;
    private int _currentGreen1 = 0;
    private int _currentGreen2 = 4;
    private GameObject _trafficLight01;
    private GameObject _trafficLight23;
    private GameObject _trafficLight45;
    public Material Red;
    public Material LitRed;
    public Material Green;
    public Material LitGreen;

    private void Start()
    {
        StartCoroutine(changeLights1());
        StartCoroutine(changeLights2());
        _trafficLight01 = GameObject.Find("Traffic light 4 Prefab 01");
        _trafficLight23 = GameObject.Find("Traffic light 4 Prefab 23");
        _trafficLight45 = GameObject.Find("Traffic light 4 Prefab 45");
    }

    private IEnumerator changeLights1()
    {
        while (true)
        {
            yield return new WaitForSeconds(_trafficWait);
            if (_currentGreen1 < 3)
            {
                _currentGreen1++;
            }
            else
            {
                _currentGreen1 = 0;
            }

            Material[] materials;
            switch (_currentGreen1)
            {
                case 0:
                    materials = _trafficLight01.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials;

                    materials[1] = Red;
                    materials[3] = LitGreen;
                    materials[4] = LitRed;
                    materials[6] = Green;
                    _trafficLight01.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials = materials;

                    materials = _trafficLight23.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials;

                    materials[1] = LitRed;
                    materials[3] = Green;
                    materials[4] = LitRed;
                    materials[6] = Green;
                    _trafficLight23.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials = materials;
                    break;
                case 1:
                    materials = _trafficLight01.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials;

                    materials[1] = LitRed;
                    materials[3] = Green;
                    materials[4] = Red;
                    materials[6] = LitGreen;
                    _trafficLight01.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials = materials;

                    materials = _trafficLight23.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials;

                    materials[1] = LitRed;
                    materials[3] = Green;
                    materials[4] = LitRed;
                    materials[6] = Green;
                    _trafficLight23.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials = materials;
                    break;
                case 2:
                    materials = _trafficLight01.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials;

                    materials[1] = LitRed;
                    materials[3] = Green;
                    materials[4] = LitRed;
                    materials[6] = Green;
                    _trafficLight01.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials = materials;

                    materials = _trafficLight23.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials;

                    materials[1] = LitRed;
                    materials[3] = Green;
                    materials[4] = Red;
                    materials[6] = LitGreen;
                    _trafficLight23.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials = materials;
                    break;
                case 3:
                    materials = _trafficLight01.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials;

                    materials[1] = LitRed;
                    materials[3] = Green;
                    materials[4] = LitRed;
                    materials[6] = Green;
                    _trafficLight01.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials = materials;

                    materials = _trafficLight23.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials;

                    materials[1] = Red;
                    materials[3] = LitGreen;
                    materials[4] = LitRed;
                    materials[6] = Green;
                    _trafficLight23.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials = materials;
                    break;
            }
            Debug.Log(_currentGreen1);
        }
    }

    private IEnumerator changeLights2()
    {
        while (true)
        {
            yield return new WaitForSeconds(_trafficWait);
            if (_currentGreen2 == 4)
            {
                _currentGreen2 = 5;
                Material[] materials = _trafficLight45.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials;
                
                materials[1] = LitRed;
                materials[3] = Green;
                materials[4] = Red;
                materials[6] = LitGreen;
                _trafficLight45.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials = materials;
            }
            else
            {
                _currentGreen2 = 4;
                Material[] materials = _trafficLight45.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials;

                materials[1] = Red;
                materials[3] = LitGreen;
                materials[4] = LitRed;
                materials[6] = Green;
                _trafficLight45.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterials = materials;
            }
            Debug.Log(_currentGreen2);
        }
    }

    public int ReturnGreen1()
    {
        return _currentGreen1;
    }

    public int ReturnGreen2()
    {
        return _currentGreen2;
    }
}
