using System.Collections;
using UnityEngine;
/// <summary>
/// Handles interaction with sleeping capsule by bed which allows for sleep and time skip
/// </summary>
public class CapsuleController : MonoBehaviour
{
    public AnimController Animator;
    public GameObject AwakeSpawn;
    public bool Asleep = false;
    private bool _called = false;

    /// <summary>
    /// When in-game tinme is at midnight, calls for the falling asleep animation and starts coroutine
    /// </summary>
    private void Update()
    {
        if (GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.ToString("HH:mm") == "00:00" && !_called)
        {
            if (!GameObject.Find("GameManager").GetComponent<GameManager>().IsInMap())
            {
                Animator.GetComponent<AnimController>().getTired();
                Asleep = true;
                StartCoroutine(Wait(10.0f, GameObject.Find("XR Origin").gameObject));
                _called = true;
            }
        }
        if (GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.ToString("HH:mm") == "07:00" && _called)
        {
            _called = false;
        }
    }
    /// <summary>
    /// Upon collision with player, calls for the falling asleep animation and starts Wait coroutine
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6 && !Asleep)
        {
            Animator.GetComponent<AnimController>().getTired();
            Asleep = true;
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
    private IEnumerator Wait(float time, GameObject other)
    {
        Animator.GetComponent<AnimController>().getTired();
        yield return new WaitForSeconds(time);
        other.transform.position = AwakeSpawn.transform.position;
        Animator.GetComponent<AnimController>().awaken();
        Asleep = false;
        GameObject.Find("GameManager").GetComponent<TimeController>().newDay();    
    }

}


