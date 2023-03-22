using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCellController : MonoBehaviour
{
    [SerializeField]
    private int loiterTime = 0;

    public int GetLoiterTime()
    {
        return loiterTime;
    }

    public void IncreaseLoiterTime(int newLoiter)
    {
        loiterTime = newLoiter;
    }
}
