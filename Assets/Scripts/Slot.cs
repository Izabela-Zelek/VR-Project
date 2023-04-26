using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
/// <summary>
/// Handles the interactions and item holding of a slot in the inventory
/// </summary>
public class Slot : MonoBehaviour
{
    public GameObject itemInSlot;
    public Image slotImage;
    public Transform attachPoint;
    Color originalColour;
    bool _canPutIn = true;
    public InputActionProperty rightSelect;


    private void Start()
    {
        slotImage = GetComponentInChildren<Image>();
        originalColour = slotImage.color;
    }
    /// <summary>
    /// Upon collision with an object, checks if item can be placed in inventory
    /// If select button is pressed, changes colour of slot to show collision
    /// If select button is let go, calls for function to add item to the slot
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!_canPutIn) return;
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
    /// <summary>
    /// Upon exit of collision with an object, if the object is the same as the one currently in the slot, calls function in current item to remove it from the slot
    /// Changes colour of slot back to original colour
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (itemInSlot != null)
        {
            if (itemInSlot == other.gameObject)
            {
                other.GetComponent<Item>().Remove();
            }
        }
        else
        {
            GameObject obj = other.gameObject;
            if (!IsItem(obj)) return;
            if (rightSelect.action.ReadValue<float>() >= 0.1f)
            {
                slotImage.color = originalColour;
            }
        }
    }
    /// <summary>
    /// Returns whether the colliding item can be put in inventory
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    bool IsItem(GameObject obj)
    {
        return obj.GetComponent<Item>();
    }
    /// <summary>
    /// Freezes the position and rotation of the item being put into inventory
    /// Sets the item to be a child of the current slot
    /// Changes colour of slot to grey to show that it is taken
    /// Turns item into a trigger to avoid it colliding with objects
    /// If it is a large item, scales the object down
    /// </summary>
    /// <param name="obj"></param>
    void InsertItem(GameObject obj)
    {
        if (!obj.GetComponent<Item>().isLarge)
        {
            obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            obj.gameObject.transform.SetParent(gameObject.transform, true);
            obj.gameObject.transform.position = attachPoint.position;
            obj.gameObject.transform.eulerAngles = obj.GetComponent<Item>().slotRotation;
            obj.GetComponent<Item>().inSlot = true;
            obj.GetComponent<Item>().currentSlot = this;
            itemInSlot = obj;
            slotImage.color = Color.gray;

            if(obj.GetComponent<BoxCollider>())
            {
                obj.GetComponent<BoxCollider>().isTrigger = true;
            }
            else
            {
                obj.GetComponent<MeshCollider>().isTrigger = true;
            }

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
            if (obj.GetComponent<BoxCollider>())
            {
                obj.GetComponent<BoxCollider>().isTrigger = true;
            }
            else
            {
                obj.GetComponent<MeshCollider>().isTrigger = true;
            }


        }
    }

    /// <summary>
    /// Removes the current item as a child of the slot
    /// Changes slot colour to original slot colour
    /// </summary>
    public void RemoveItem()
    {
        itemInSlot.transform.SetParent(null, true);
        itemInSlot = null;
        slotImage.color = originalColour;
    }

    public void SetPutIn(bool put)
    {
        _canPutIn = put;
    }
}
