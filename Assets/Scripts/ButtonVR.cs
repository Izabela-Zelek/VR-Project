using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonVR : MonoBehaviour
{
    public int price;
    public GameObject button;
    public GameObject boughtObject;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    private GameObject presser;
    private AudioSource sound;
    bool isPressed;
    void Start()
    {
        sound = GetComponent<AudioSource>();
        isPressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isPressed && other.gameObject.layer == 11)
        {
            button.transform.localPosition = new Vector3(0, 0.02f, 0);
            presser = other.gameObject;
            onPress.Invoke();
            sound.Play();
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject)
        {
            button.transform.localPosition = new Vector3(0, 0.035f, 0);
            onRelease.Invoke();
            isPressed = false;
        }
    }

    public void SpawnObject()
    {
        if (price < GameObject.Find("GameManager").GetComponent<GameManager>().GetMoney())
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().UpdateMoney(GameObject.Find("GameManager").GetComponent<GameManager>().GetMoney() - price);
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y - 0.205f, transform.position.z - 0.24f);
            GameObject boughtItem = Instantiate(boughtObject, spawnPos, Quaternion.identity);
        }
    }
}
