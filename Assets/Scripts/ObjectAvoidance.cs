using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the avoidance of objects for the NPCs
/// </summary>
public class ObjectAvoidance : MonoBehaviour
{
    private Vector3 avoidance_force;
    private float max_avoidance = 60;
    private List<string> _metNPC = new List<string>();
    /// <summary>
    /// Upon colliding with a trigger object
    /// If object is an NPC, generates a random chance the NPC will stop for a chat. Adds the met NPC into a list so as to not be stuck in a loop
    /// If other object, calculates an avoidance force by reflecting the direction the NPC is coming from
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            if (other.gameObject.GetComponent<PathMover>() && transform.parent.gameObject.GetComponent<PathMover>())
            {
                if (other.gameObject.GetComponent<PathMover>().isActiveAndEnabled && transform.parent.gameObject.GetComponent<PathMover>().isActiveAndEnabled)
                {
                    int chanceTalk = Random.Range(1, 3);

                    if (chanceTalk == 1 && !_metNPC.Contains(other.name))
                    {
                        int _talkLength = Random.Range(4, 11);
                        _metNPC.Add(other.name);
                        other.transform.Find("CollisionCheck").GetComponent<ObjectAvoidance>().AddNPC(transform.parent.name);
                        Vector3 thisDirection = other.transform.position - transform.position;

                        StartCoroutine(transform.parent.GetComponent<PathMover>().Talk(_talkLength, thisDirection));
                        StartCoroutine(other.GetComponent<PathMover>().Talk(_talkLength, -thisDirection));
                    }
                }
            }
        }
        else if (other.tag != "NPCCollision" && other.tag != "Ground" && other.tag != "Plane" && other.gameObject.layer != LayerMask.NameToLayer("Curb") && other.tag != "Decor" && other.tag != "Grass")
        {
            if (transform.parent.GetComponent<Rigidbody>().velocity != Vector3.zero)
            {
                avoidance_force = Vector3.Reflect(transform.forward, other.transform.position).normalized;
                avoidance_force = avoidance_force.normalized * max_avoidance;

                transform.parent.GetComponent<NPCContoller>().Avoid(avoidance_force);
            }
        }
    }
    /// <summary>
    /// Upon colliding with a collider object
    /// If object is an NPC, generates a random chance the NPC will stop for a chat. Adds the met NPC into a list so as to not be stuck in a loop
    /// If other object, calculates an avoidance force by reflecting the direction the NPC is coming from
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            if (collision.gameObject.GetComponent<PathMover>() && transform.parent.gameObject.GetComponent<PathMover>())
            {
                if (collision.gameObject.GetComponent<PathMover>().isActiveAndEnabled && transform.parent.gameObject.GetComponent<PathMover>().isActiveAndEnabled)
                {
                    Debug.Log(collision.collider.name);

                    int chanceTalk = Random.Range(1, 6);

                    if (chanceTalk == 1 && !_metNPC.Contains(collision.collider.name))
                    {
                        int _talkLength = Random.Range(4, 11);
                        _metNPC.Add(collision.collider.name);
                        collision.collider.transform.Find("CollisionCheck").GetComponent<ObjectAvoidance>().AddNPC(transform.parent.name);
                        Vector3 thisDirection = collision.collider.transform.position - transform.position;

                        StartCoroutine(transform.parent.GetComponent<PathMover>().Talk(_talkLength, thisDirection));
                        StartCoroutine(collision.collider.GetComponent<PathMover>().Talk(_talkLength, -thisDirection));
                    }
                }
            }
        }
        else if(collision.collider.tag != "NPCCollision" && collision.collider.tag != "Ground" && collision.collider.tag != "Plane" && collision.gameObject.layer != LayerMask.NameToLayer("Curb") && collision.collider.tag != "Decor" && collision.collider.tag != "Grass")
        {
            Debug.Log(collision.collider.name);
            if (transform.parent.GetComponent<Rigidbody>().velocity != Vector3.zero)
            {
                avoidance_force = Vector3.Reflect(transform.forward, collision.transform.position).normalized;
                avoidance_force = avoidance_force.normalized * max_avoidance;

                transform.parent.GetComponent<NPCContoller>().Avoid(avoidance_force);
            }
        }
    }
    /// <summary>
    /// Adds passed in npc name into a list of met NPCs
    /// </summary>
    /// <param name="name"></param>
    public void AddNPC(string name)
    {
        _metNPC.Add(name);
    }
}
