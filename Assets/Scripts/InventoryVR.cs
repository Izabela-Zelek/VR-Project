using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class InventoryVR : MonoBehaviour
{
    public InputActionProperty leftActivate;

    public GameObject Inventory;
    public GameObject Anchor;
    bool UIActive;

    private void Start()
    {
        Inventory.SetActive(false);
        UIActive = false;

    }

    private void Update()
    {
       
        if (leftActivate.action.ReadValue<float>() > 0.1f)
        {
            UIActive = true;
            Inventory.SetActive(UIActive);
        }
        else
        {
            UIActive = false;
            Inventory.SetActive(UIActive);
        }
        if (UIActive)
        {
            Inventory.transform.position = Anchor.transform.position;
            Inventory.transform.eulerAngles = new Vector3(Anchor.transform.eulerAngles.x + 15, Anchor.transform.eulerAngles.y, 0);
        }
    }
}