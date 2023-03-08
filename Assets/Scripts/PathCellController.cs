using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCellController : MonoBehaviour
{
    [SerializeField]
    private int loiterTime;

    public int GetLoiterTime()
    {
        return loiterTime;
    }
}
