using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLampController : MonoBehaviour
{
    private TimeController timeManager;

    private void Start()
    {
        timeManager = GameObject.Find("GameManager").GetComponent<TimeController>();
    }
    private void Update()
    {
        if(timeManager.currentTime.Hour == 20 || timeManager.currentTime.Hour == 3)
        {
            for(int i = 0; i < transform.childCount;i++)
            {
                transform.GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(true);
            }
        }
        if (timeManager.currentTime.Hour == 7)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
