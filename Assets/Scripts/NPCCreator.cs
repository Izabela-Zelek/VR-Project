using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCreator : MonoBehaviour
{
    private GameObject newNPC;
    private int pathId = -1;

    public void setNPC(GameObject npc)
    {
        newNPC = npc;
        if (pathId != -1)
        {
            newNPC.GetComponent<PathMover>().SetDefaultPath(pathId);
        }
        else
        {
            int rand = Random.Range(1, 4);
            newNPC.GetComponent<PathMover>().SetDefaultPath(rand);
        }
    }

    public void setPath(int npcPath)
    {
        pathId = npcPath;
    }
}
