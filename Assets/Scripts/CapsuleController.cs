using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    public AnimController animator;
    public GameObject awakeSpawn;
    public bool asleep = false;

    private void Update()
    {
        if (GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.ToString("HH:mm") == "01:00")
        {
            animator.GetComponent<AnimController>().getTired();
            asleep = true;
            StartCoroutine(Wait(10.0f, GameObject.Find("XR Origin").gameObject));
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6 && !asleep)
        {
            animator.GetComponent<AnimController>().getTired();
            asleep = true;
            StartCoroutine(Wait(10.0f, other.gameObject));
        }
    }
    IEnumerator Wait(float time, GameObject other)
    {
        yield return new WaitForSeconds(time);
        other.transform.position = awakeSpawn.transform.position;
        animator.GetComponent<AnimController>().awaken();
        asleep = false;
    }

}


