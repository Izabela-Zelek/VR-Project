using System.Collections;
using UnityEngine;
/// <summary>
/// Handles interaction with sleeping capsule by bed which allows for sleep and time skip
/// </summary>
public class CapsuleController : MonoBehaviour
{
    public AnimController animator;
    public GameObject awakeSpawn;
    public bool asleep = false;
    private bool called = false;

    /// <summary>
    /// When in-game tinme is at midnight, calls for the falling asleep animation and starts coroutine
    /// </summary>
    private void Update()
    {
        if (GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.ToString("HH:mm") == "00:00" && !called)
        {
            if (!GameObject.Find("GameManager").GetComponent<GameManager>().IsInMap())
            {
                animator.GetComponent<AnimController>().getTired();
                asleep = true;
                StartCoroutine(Wait(10.0f, GameObject.Find("XR Origin").gameObject));
                called = true;
            }
        }
        if (GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.ToString("HH:mm") == "07:00" && called)
        {
            called = false;
        }
    }
    /// <summary>
    /// Upon collision with player, calls for the falling asleep animation and starts Wait coroutine
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6 && !asleep)
        {
            animator.GetComponent<AnimController>().getTired();
            asleep = true;
            StartCoroutine(Wait(10.0f, other.gameObject));
        }
    }
    /// <summary>
    /// Plays falling asleep animation, waits the designated time
    /// Changes player position to by bedside
    /// Plays waking up animation
    /// Increments the day number
    /// </summary>
    /// <param name="time"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    IEnumerator Wait(float time, GameObject other)
    {
        animator.GetComponent<AnimController>().getTired();
        yield return new WaitForSeconds(time);
        other.transform.position = awakeSpawn.transform.position;
        animator.GetComponent<AnimController>().awaken();
        asleep = false;
        GameObject.Find("GameManager").GetComponent<TimeController>().newDay();    
    }

}


