using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    private int _trafficWait = 10;
    private int _currentGreen1 = 0;
    private int _currentGreen2 = 4;

    private void Start()
    {
        StartCoroutine(changeLights1());
        StartCoroutine(changeLights2());
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
            }
            else
            {
                _currentGreen2 = 4;
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
