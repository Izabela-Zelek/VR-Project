using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    public AnimController animator;
    public GameObject awakeSpawn;
    public bool asleep = false;
    private bool called = false;

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
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6 && !asleep)
        {
            animator.GetComponent<AnimController>().getTired();
            asleep = true;
            StartCoroutine(Wait(0.5f, other.gameObject));
        }
    }
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


