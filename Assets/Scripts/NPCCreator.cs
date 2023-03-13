using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCreator : MonoBehaviour
{
    private GameObject newNPC;
    private int pathId = -1;
    private int oldChosen = -1;
    private GameObject path;
    private int closestPath = -1;
    float closestDistance = float.MaxValue;

    private void Start()
    {
        path = GameObject.Find("Paths");

    }
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

    public void setPath(int npcPath)
    {
        pathId = npcPath;

        GameObject chosenPath = GameObject.Find("Path" + npcPath);
        for (int i = 0; i < chosenPath.transform.childCount; i++)
        {
            chosenPath.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }

        if(oldChosen != -1)
        {
            chosenPath = GameObject.Find("Path" + oldChosen);
            for (int i = 0; i < chosenPath.transform.childCount; i++)
            {
                chosenPath.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }
        
        oldChosen = npcPath;
    }

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
                }
            }
        }
    }
}
