using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the stopping of vehicles when a car, NPC or player is in front of it, plays honk audio
/// </summary>
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
    /// <summary>
    /// If constantly colliding with collider object, stops movement and plays honking sound if its an NPC or Player
    /// </summary>
    /// <param name="collision"></param>
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
    /// <summary>
    /// If colliding with collider object, stops movement and plays honking sound if its an NPC or Player
    /// </summary>
    /// <param name="collision"></param>
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
    /// <summary>
    /// If not colliding with an object anymore, resumes movement
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Curb") && collision.gameObject.layer != LayerMask.NameToLayer("Ground") && collision.gameObject.layer != LayerMask.NameToLayer("Boundary") && collision.gameObject.layer != LayerMask.NameToLayer("Lights") && collision.transform.tag != "EntryPoints")
        {
            transform.parent.GetComponent<VehicleMover>().setMove(true);
        }
    }
    /// <summary>
    /// If constantly colliding with trigger object, stops movement and plays honking sound if its an NPC or Player
    /// </summary>
    /// <param name="other"></param>
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

    /// <summary>
    /// If colliding with trigger object, stops movement and plays honking sound if its an NPC or Player
    /// </summary>
    /// <param name="other"></param>
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
    /// <summary>
    ///  If not colliding with a trigger object anymore, resumes movement
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Curb") && collision.gameObject.layer != LayerMask.NameToLayer("Ground") && collision.gameObject.layer != LayerMask.NameToLayer("Boundary") && collision.gameObject.layer != LayerMask.NameToLayer("Lights") && collision.transform.tag != "EntryPoints")
        {
            transform.parent.GetComponent<VehicleMover>().setMove(true);
        }
    }
}
