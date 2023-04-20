using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCollision : MonoBehaviour
{
    private AudioSource _honk;

    private void Start()
    {
        _honk = GameObject.Find("AudioManager").transform.Find("Honk").GetComponent<AudioSource>();
    }
    private void Update()
    {
        transform.parent.GetComponent<VehicleMover>().setMove(true);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Curb") && collision.gameObject.layer != LayerMask.NameToLayer("Ground") && collision.gameObject.layer != LayerMask.NameToLayer("Boundary") && collision.gameObject.layer != LayerMask.NameToLayer("Lights") && collision.transform.tag != "EntryPoints")
        {
            transform.parent.GetComponent<VehicleMover>().setMove(false);

            if (collision.gameObject.layer == LayerMask.NameToLayer("NPC") || collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _honk.Play();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Curb") && collision.gameObject.layer != LayerMask.NameToLayer("Ground") && collision.gameObject.layer != LayerMask.NameToLayer("Boundary") && collision.gameObject.layer != LayerMask.NameToLayer("Lights") && collision.transform.tag != "EntryPoints")
        {
            transform.parent.GetComponent<VehicleMover>().setMove(false);

            if (collision.gameObject.layer == LayerMask.NameToLayer("NPC") || collision.gameObject.layer == LayerMask.NameToLayer("NPC"))
             {
                _honk.Play();
            }

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Curb") && collision.gameObject.layer != LayerMask.NameToLayer("Ground") && collision.gameObject.layer != LayerMask.NameToLayer("Boundary") && collision.gameObject.layer != LayerMask.NameToLayer("Lights") && collision.transform.tag != "EntryPoints")
        {
            transform.parent.GetComponent<VehicleMover>().setMove(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Curb") && other.gameObject.layer != LayerMask.NameToLayer("Ground") && other.gameObject.layer != LayerMask.NameToLayer("Boundary") && other.gameObject.layer != LayerMask.NameToLayer("Lights") && other.transform.tag != "EntryPoints")
        {
            transform.parent.GetComponent<VehicleMover>().setMove(false);

            if (other.gameObject.layer == LayerMask.NameToLayer("NPC")|| other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _honk.Play();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Curb") && other.gameObject.layer != LayerMask.NameToLayer("Ground") && other.gameObject.layer != LayerMask.NameToLayer("Boundary") && other.gameObject.layer != LayerMask.NameToLayer("Lights") && other.transform.tag != "EntryPoints")
        {
            transform.parent.GetComponent<VehicleMover>().setMove(false);

            if (other.gameObject.layer == LayerMask.NameToLayer("NPC") || other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _honk.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Curb") && collision.gameObject.layer != LayerMask.NameToLayer("Ground") && collision.gameObject.layer != LayerMask.NameToLayer("Boundary") && collision.gameObject.layer != LayerMask.NameToLayer("Lights") && collision.transform.tag != "EntryPoints")
        {
            transform.parent.GetComponent<VehicleMover>().setMove(true);
        }
    }
}
