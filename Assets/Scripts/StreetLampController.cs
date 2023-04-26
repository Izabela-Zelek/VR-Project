using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the turning on and off of the street lamps at appropriate times
/// </summary>
public class StreetLampController : MonoBehaviour
{
    private TimeController _timeManager;

    private void Start()
    {
        _timeManager = GameObject.Find("GameManager").GetComponent<TimeController>();
    }
    /// <summary>
    /// Turns on each streetlamp at beginning of game and at 8pm
    /// Turns off each streetlamp at 7am
    /// </summary>
    private void Update()
    {
        if(_timeManager.CurrentTime.Hour == 20 || _timeManager.CurrentTime.Hour == 3)
        {
            for(int i = 0; i < transform.childCount;i++)
            {
                transform.GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(true);
            }
        }
        if (_timeManager.CurrentTime.Hour == 7)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
