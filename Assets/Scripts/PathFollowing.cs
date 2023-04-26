using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
/// <summary>
/// UNUSED Handles the ability to draw the custom paths with a mouse - left in for archival and version control purposes. Provides a record of the previous versions and iterations of the project
/// </summary>
public class PathFollowing : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private List<Vector3> _points = new List<Vector3>();

    public Action<IEnumerable<Vector3>> OnNewPathCreated = delegate { };

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Casts raycast onto world, adds each point the hit touches if the distance between it and the previous one is greater than 1
    /// Clears the path is screen is clicked again
    /// </summary>
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

    /// <summary>
    /// Calculates distance between current point and the point before it
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private float DistanceToLastPoint(Vector3 point)
    {
        if(!_points.Any())
        {
            return Mathf.Infinity;
        }
        return Vector3.Distance(_points.Last(), point);
    }
}
