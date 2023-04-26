using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles the functionality of the hoe object
/// </summary>
public class HoeScript : MonoBehaviour
{
    public GameObject dirtPatch;
    public Transform tip;
    public GameObject farm;
    private List<Vector3> dirtPos = new List<Vector3>();
    private XRDirectInteractor rightInteractor;
    private XRDirectInteractor leftInteractor;
    private bool free = true;
    private AudioSource _hoe;
    private void Start()
    {
        rightInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(2).GetComponent<XRDirectInteractor>();
        leftInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(1).GetComponent<XRDirectInteractor>();
        _hoe = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Spawns raycast at tip of hoe
    /// If hoe collides with another planting fied, disallows spawning of another
    /// If tip of hoe collides with gameobjects that aren't farm land, disallows spawning of planting field
    /// If allowed, plays tilling audio, spawns planting field, adds position of new field to list of positions
    /// </summary>
    private void Update()
    {
        if ((rightInteractor.interactablesSelected.Count > 0 && rightInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()) || (leftInteractor.interactablesSelected.Count > 0 && leftInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()))
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
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.up, out hit))
                    {
                        if (hit.collider.name != "FarmLand")
                        {
                            free = false;
                        }
                    }
                    if (free)
                    {
                        _hoe.Play();
                        Instantiate(dirtPatch, pos, Quaternion.identity, farm.transform);
                        dirtPos.Add(pos);
                    }
                    free = true;
                }
            }
        }
    }
}
