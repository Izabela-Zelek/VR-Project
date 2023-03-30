using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCellController : MonoBehaviour
{
    [SerializeField]
    private int loiterTime = 0;
    private float startTime = 0;

    public int GetLoiterTime()
    {
        return loiterTime;
    }

    public void IncreaseLoiterTime(int newLoiter)
    {
        loiterTime = newLoiter;
    }
    private void Update()
    {
        if(loiterTime < 0)
        {
            loiterTime = 0;
        }
        if (startTime < 0)
        {
            startTime = 0;
        }
        if (loiterTime > 0 && startTime > 0)
        {
            loiterTime = 0;
        }
    }
}
