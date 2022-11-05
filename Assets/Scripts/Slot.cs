using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Slot : MonoBehaviour
{
    public GameObject itemInSlot;
    public Image slotImage;
    public Transform attachPoint;
    Color originalColour;

    public InputActionProperty rightSelect;


    private void Start()
    {
        slotImage = GetComponentInChildren<Image>();
        originalColour = slotImage.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (itemInSlot != null) return;
        GameObject obj = other.gameObject;
        if (!IsItem(obj)) return;
        if (rightSelect.action.ReadValue<float>() >= 0.1f && obj.GetComponent<Item>().isHeld && other.gameObject.GetComponent<Item>().inSlot == false)
        { 
            slotImage.color = new Color(0.6156863f, 0.4156863f, 0.5215687f); 
        }

        if (rightSelect.action.ReadValue<float>() == 0 && obj.GetComponent<Item>().isHeld && other.gameObject.GetComponent<Item>().inSlot == false)
        {
            InsertItem(obj);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (itemInSlot != null) return;
        GameObject obj = other.gameObject;
        if (!IsItem(obj)) return;
        if (rightSelect.action.ReadValue<float>() >= 0.1f)
        { 
            slotImage.color = originalColour; 
        }
    }
    bool IsItem(GameObject obj)
    {
        return obj.GetComponent<Item>();
    }

    void InsertItem(GameObject obj)
    {
        if (!obj.GetComponent<Item>().isLarge)
        {
            obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            obj.transform.SetParent(gameObject.transform, true);
            obj.transform.position = attachPoint.position;
            obj.transform.eulerAngles = obj.GetComponent<Item>().slotRotation;
            obj.GetComponent<Item>().inSlot = true;
            obj.GetComponent<Item>().currentSlot = this;
            itemInSlot = obj;
            slotImage.color = Color.gray;
            obj.GetComponent<BoxCollider>().isTrigger = true;

        }
        else
        {
            obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            obj.gameObject.transform.SetParent(gameObject.transform, true);
            Vector3 scale = obj.gameObject.transform.localScale;

            scale.Set(0.25f, 0.25f, 0.25f);

            obj.gameObject.transform.localScale = scale;
            obj.gameObject.transform.position = attachPoint.position;
            obj.gameObject.transform.rotation = Quaternion.Euler(obj.GetComponent<Item>().slotRotation);
            obj.GetComponent<Item>().inSlot = true;
            obj.GetComponent<Item>().currentSlot = this;
            itemInSlot = obj;
            slotImage.color = Color.gray;
            obj.GetComponent<MeshCollider>().isTrigger = true;


        }
    }

    //void InsertLargeItem(GameObject obj)
    //{
    //    obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    //    obj.gameObject.transform.SetParent(gameObject.transform, true);
    //    Vector3 scale = obj.gameObject.transform.localScale;

    //    scale.Set(0.25f, 0.25f, 0.25f);

    //    obj.gameObject.transform.localScale = scale;
    //    obj.gameObject.transform.position = attachPoint.position;
    //    obj.gameObject.transform.rotation = Quaternion.Euler(obj.GetComponent<Item>().slotRotation);
    //    obj.GetComponent<Item>().inSlot = true;
    //    obj.GetComponent<Item>().currentSlot = this;
    //    itemInSlot = obj;
    //    slotImage.color = Color.gray;
    //}

    public void RemoveItem()
    {
        itemInSlot.transform.SetParent(null, true);
        itemInSlot = null;
        slotImage.color = originalColour;
    }

}
