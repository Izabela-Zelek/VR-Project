using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCellController : MonoBehaviour
{
    [SerializeField]
    private int _loiterTime = 0;
    private float _startTime = 0;
    [SerializeField]
    private bool atShop = false;
    public int GetLoiterTime()
    {
        return _loiterTime;
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

    public bool GetAtShop()
    {
        return atShop;
    }
}
