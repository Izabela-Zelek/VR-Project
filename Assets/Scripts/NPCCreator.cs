using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the spawning of new NPCs when selecting the option in the Map/Level Editor
/// </summary>
public class NPCCreator : MonoBehaviour
{
    private GameObject _newNPC;
    private int _pathId = -1;
    private int _oldChosen = -1;
    private GameObject _path;
    private int _closestPath = -1;
    private float _closestDistance = float.MaxValue;

    private void Start()
    {
        _path = GameObject.Find("Paths");

    }
    /// <summary>
    /// Finds the closest path to the placed NPC
    /// </summary>
    /// <param name="npc"></param>
    public void SetNPC(GameObject npc)
    {
        _newNPC = npc;
        if (_pathId != -1)
        {
            _newNPC.GetComponent<PathMover>().SetDefaultPath(_pathId);
        }
        else
        {
            FindClosestPath(npc.transform.position);
            _newNPC.GetComponent<PathMover>().SetDefaultPath(_closestPath);
            ClearMap();

        }
    }

    /// <summary>
    /// Turns on markers for the selected path so that it displays on the Map/Level Editor
    /// </summary>
    /// <param name="npcPath"></param>
    public void SetPath(int npcPath)
    {
        _pathId = npcPath;

        GameObject chosenPath = GameObject.Find("Path" + npcPath);
        for (int i = 0; i < chosenPath.transform.childCount; i++)
        {
            chosenPath.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }

        if(_oldChosen != -1 && npcPath != _oldChosen)
        {
            chosenPath = GameObject.Find("Path" + _oldChosen);
            for (int i = 0; i < chosenPath.transform.childCount; i++)
            {
                chosenPath.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }
        
        _oldChosen = npcPath;
    }

    /// <summary>
    /// Hides the markers for each cell of the passed in path id
    /// </summary>
    /// <param name="npcPath"></param>
    public void HidePath(int npcPath)
    {
        GameObject chosenPath = GameObject.Find("Path" + npcPath);
        for (int i = 0; i < chosenPath.transform.childCount; i++)
        {
            chosenPath.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Hides all markers of each cell of every path
    /// </summary>
    public void HideAllPath()
    {
        GameObject pathParent = GameObject.Find("Paths");

        for (int i = 0; i < pathParent.transform.childCount;i++)
        {
            pathParent.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Clears the markers for the current path if the id is not -1
    /// </summary>
    private void ClearMap()
    {
        if (_pathId != -1)
        {
            GameObject chosenPath = GameObject.Find("Path" + _pathId);
            for (int i = 0; i < chosenPath.transform.childCount; i++)
            {
                chosenPath.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }

        _pathId = -1;
        _oldChosen = -1;
        _closestPath = -1;
        _closestDistance = float.MaxValue;
}
    /// <summary>
    /// Loops through all possible paths and finds the path closest to the placed NPC
    /// </summary>
    /// <param name="npcPos"></param>
    private void FindClosestPath(Vector3 npcPos)
    {
        for(int i = 0; i < _path.transform.childCount; i++)
        {
            Transform pathObject = _path.transform.GetChild(i);
            for (int j = 0; j < pathObject.childCount; j++)
            {
                float distance = Vector3.Distance(pathObject.GetChild(j).position, npcPos);
                if(distance < _closestDistance && j != pathObject.childCount - 1)
                {
                    _closestDistance = distance;
                    _closestPath = i + 1;

                    if(_closestPath == 5)
                    {
                        _closestPath = 6;
                    }
                    else if(_closestPath == 4)
                    {
                        _closestPath = 5;
                    }
                }
            }
        }
    }
}
