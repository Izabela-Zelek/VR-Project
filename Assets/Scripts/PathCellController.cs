using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the cells in each possible NPC path
/// Specifies the amount of time spent loitering at the cell, 
/// </summary>
public class PathCellController : MonoBehaviour
{
    [SerializeField]
    private int _loiterTime = 0;
    [SerializeField]
    private bool _atShop = false;

    private float _startTime = 0;

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
    /// <summary>
    /// Retunrs whether or not the cell is at a shop
    /// </summary>
    /// <returns></returns>
    public bool GetAtShop()
    {
        return _atShop;
    }
}
