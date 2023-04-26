using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// UNUSED Handles the drawing of a line to follow the custom path - left in for archival and version control purposes. Provides a record of the previous versions and iterations of the project
/// </summary>
public class PathDecor : MonoBehaviour
{
    public List<Transform> Waypoints;
    public Color DebugColour = Color.white;

    [SerializeField]
    private bool _alwaysDrawPath;
    [SerializeField]
    private bool _drawAsLoop;
    [SerializeField]
    private bool _drawNumbers;

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (_alwaysDrawPath)
        {
            DrawPath();
        }
    }

    /// <summary>
    /// Using each waypoint, draws the path on screen
    /// </summary>
    public void DrawPath()
    {
        for (int i = 0; i < Waypoints.Count; i++)
        {
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 30;
            labelStyle.normal.textColor = DebugColour;
            if (_drawNumbers)
                Handles.Label(Waypoints[i].position, i.ToString(), labelStyle);

            if (i >= 1)
            {
                Gizmos.color = DebugColour;
                Gizmos.DrawLine(Waypoints[i - 1].position, Waypoints[i].position);

                if (_drawAsLoop)
                    Gizmos.DrawLine(Waypoints[Waypoints.Count - 1].position, Waypoints[0].position);

            }
        }
    }


    public void OnDrawGizmosSelected()
    {
        if (_alwaysDrawPath)
            return;
        else
            DrawPath();
    }
#endif
}
