using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimalShop : MonoBehaviour
{
    public int price;
    public GameObject button;
    public GameObject boughtObject;
    public Transform pos;
    private AudioSource sound;
    public UnityEvent onPress;
    public UnityEvent onRelease;
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
        if (price < GameObject.Find("GameManager").GetComponent<GameManager>().GetMoney() && GameObject.Find("GameManager").GetComponent<GameManager>().shopOpen)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().UpdateMoney(GameObject.Find("GameManager").GetComponent<GameManager>().GetMoney() - price);
            GameObject boughtItem = Instantiate(boughtObject, pos.position, Quaternion.identity,GameObject.Find("Animals").transform);
        }
    }
}
