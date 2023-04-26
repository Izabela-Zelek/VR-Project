using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the spawning of new NPCs when selecting the option in the Map/Level Editor
/// </summary>
public class NPCCreator : MonoBehaviour
{
    private GameObject newNPC;
    private int pathId = -1;
    private int oldChosen = -1;
    private GameObject path;
    private int closestPath = -1;
    float closestDistance = float.MaxValue;
    /// <summary>
    /// Finds the closest path to the placed NPC
    /// </summary>
    /// <param name="npc"></param>
    private void Start()
    {
        path = GameObject.Find("Paths");

    }
    /// <summary>
    /// Finds the closest path to the placed NPC
    /// </summary>
    /// <param name="npc"></param>
    public void setNPC(GameObject npc)
    {
        newNPC = npc;
        if (pathId != -1)
        {
            newNPC.GetComponent<PathMover>().SetDefaultPath(pathId);
        }
        else
        {
            FindClosestPath(npc.transform.position);
            newNPC.GetComponent<PathMover>().SetDefaultPath(closestPath);
            ClearMap();

        }
    }
    /// <summary>
    /// Turns on markers for the selected path so that it displays on the Map/Level Editor
    /// </summary>
    /// <param name="npcPath"></param>
    public void setPath(int npcPath)
    {
        pathId = npcPath;

        GameObject chosenPath = GameObject.Find("Path" + npcPath);
        for (int i = 0; i < chosenPath.transform.childCount; i++)
        {
            chosenPath.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }

        if(oldChosen != -1 && npcPath != oldChosen)
        {
            chosenPath = GameObject.Find("Path" + oldChosen);
            for (int i = 0; i < chosenPath.transform.childCount; i++)
            {
                chosenPath.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }
        
        oldChosen = npcPath;
    }
    /// <summary>
    /// Hides the markers for each cell of the passed in path id
    /// </summary>
    /// <param name="npcPath"></param>
    public void hidePath(int npcPath)
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

        for (int i = 0; i < pathParent.transform.childCount; i++)
        {
            pathParent.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Clears the markers for the current path if the id is not -1
    /// </summary>
    public void ClearMap()
    {
        if (pathId != -1)
        {
            GameObject chosenPath = GameObject.Find("Path" + pathId);
            for (int i = 0; i < chosenPath.transform.childCount; i++)
            {
                chosenPath.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }

        pathId = -1;
        oldChosen = -1;
        closestPath = -1;
        closestDistance = float.MaxValue;
}
    /// <summary>
    /// Loops through all possible paths and finds the path closest to the placed NPC
    /// </summary>
    /// <param name="npcPos"></param>
    private void FindClosestPath(Vector3 npcPos)
    {
        for(int i = 0; i < path.transform.childCount; i++)
        {
            Transform pathObject = path.transform.GetChild(i);
            for (int j = 0; j < pathObject.childCount; j++)
            {
                float distance = Vector3.Distance(pathObject.GetChild(j).position, npcPos);
                if(distance < closestDistance && j != pathObject.childCount - 1)
                {
                    closestDistance = distance;
                    closestPath = i + 1;

                    if(closestPath == 5)
                    {
                        closestPath = 6;
                    }
                    else if(closestPath == 4)
                    {
                        closestPath = 5;
                    }
                }
            }
        }
    }
}
