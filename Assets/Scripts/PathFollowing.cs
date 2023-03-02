using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PathFollowing : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private List<Vector3> _points = new List<Vector3>();

    public Action<IEnumerable<Vector3>> OnNewPathCreated = delegate { };

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            _points.Clear();
        }
        if (Input.GetButton("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray,out hitInfo))
            {
                if(DistanceToLastPoint(hitInfo.point) > 1f)
                {
                    _points.Add(hitInfo.point);

                    _lineRenderer.positionCount = _points.Count;
                    _lineRenderer.SetPositions(_points.ToArray());
                }
            }
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            OnNewPathCreated(_points);
        }
    }

    private float DistanceToLastPoint(Vector3 point)
    {
        if(!_points.Any())
        {
            return Mathf.Infinity;
        }
        return Vector3.Distance(_points.Last(), point);
    }
}
