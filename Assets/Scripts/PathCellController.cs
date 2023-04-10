using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCellController : MonoBehaviour
{
    [SerializeField]
    private int _loiterTime = 0;
    private float _startTime = 0;
    [SerializeField]
    private int _startWalkTime = 8;
    [SerializeField]
    private int _endWalkTime = -1;
    public int GetLoiterTime()
    {
        return _loiterTime;
    }
    public float GetStartTime()
    {
        return _startWalkTime;
    }
    public float GetEndTime()
    {
        return _endWalkTime;
    }

    public void IncreaseLoiterTime(int newLoiter)
    {
        _loiterTime = newLoiter;
    }
    private void Update()
    {
        if(_loiterTime < 0)
        {
            _loiterTime = 0;
        }
        if (_startTime < 0)
        {
            _startTime = 0;
        }
        if (_loiterTime > 0 && _startTime > 0)
        {
            _loiterTime = 0;
        }
    }
}
