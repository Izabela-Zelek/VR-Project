using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : MonoBehaviour
{
    private bool colliding = false;
    private AudioSource _chop;

    private void Start()
    {
        _chop = GetComponent<AudioSource>();
    }
    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up, out hit, 0.1f))
        {
            if (hit.collider.tag == "Tree" && !colliding)
            {
                GameObject tree = hit.collider.gameObject;
                while (tree.transform.parent.tag != "Parent")
                {
                    tree = tree.transform.parent.gameObject;
                }
                tree.GetComponent<TreeController>().MinusLife();
                _chop.Play();
                colliding = true;
            }
        }
        else
        {
            colliding = false;
        }
    }
}
