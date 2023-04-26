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
        if (alwaysDrawPath)
        {
            DrawPath();
        }
    }

    /// <summary>
    /// Using each waypoint, draws the path on screen
    /// </summary>
    public void DrawPath()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 30;
            labelStyle.normal.textColor = debugColour;
            if (drawNumbers)
                Handles.Label(waypoints[i].position, i.ToString(), labelStyle);

            if (i >= 1)
            {
                Gizmos.color = debugColour;
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);

                if (drawAsLoop)
                    Gizmos.DrawLine(waypoints[waypoints.Count - 1].position, waypoints[0].position);

            }
        }
    }


    public void OnDrawGizmosSelected()
    {
        if (alwaysDrawPath)
            return;
        else
            DrawPath();
    }
#endif
}
