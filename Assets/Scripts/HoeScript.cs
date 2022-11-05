using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoeScript : MonoBehaviour
{
    public GameObject dirtPatch;
    public Transform tip;
    public GameObject farm;
    private List<Vector3> dirtPos = new List<Vector3>();
    private bool free = true;

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (tip.transform.position.y <= 0.17f)
            {
                Vector3 pos = new Vector3(Mathf.Round(tip.position.x), 0, Mathf.Round(tip.position.z));
                for (int i = 0; i < dirtPos.Count; i++)
                {
                    if (pos == dirtPos[i])
                    {
                        free = false;
                    }
                }
                if (free)
                {
                    Instantiate(dirtPatch, pos, Quaternion.identity, farm.transform);
                    dirtPos.Add(pos);
                }
                free = true;
            }
        }
    }
}
