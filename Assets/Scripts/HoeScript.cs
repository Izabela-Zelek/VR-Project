using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles the functionality of the hoe object
/// </summary>
public class HoeScript : MonoBehaviour
{
    public GameObject DirtPatch;
    public Transform Tip;
    public GameObject Farm;

    private List<Vector3> _dirtPos = new List<Vector3>();
    private XRDirectInteractor _rightInteractor;
    private XRDirectInteractor _leftInteractor;
    private bool _free = true;
    private AudioSource _hoe;

    private void Start()
    {
        _rightInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(2).GetComponent<XRDirectInteractor>();
        _leftInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(1).GetComponent<XRDirectInteractor>();
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
        if ((_rightInteractor.interactablesSelected.Count > 0 && _rightInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()) || (_leftInteractor.interactablesSelected.Count > 0 && _leftInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()))
        { 
            if (gameObject.activeSelf)
            {
                if (Tip.transform.position.y <= 0.17f)
                {
                    Vector3 pos = new Vector3(Mathf.Round(Tip.position.x), 0, Mathf.Round(Tip.position.z));
                    for (int i = 0; i < _dirtPos.Count; i++)
                    {
                        if (pos == _dirtPos[i])
                        {
                            _free = false;
                        }
                    }
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.up, out hit))
                    {
                        if (hit.collider.name != "FarmLand")
                        {
                            _free = false;
                        }
                    }
                    if (_free)
                    {
                        _hoe.Play();
                        Instantiate(DirtPatch, pos, Quaternion.identity, Farm.transform);
                        _dirtPos.Add(pos);
                    }
                    _free = true;
                }
            }
        }
    }
}
