using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Old unused script - left in for archival and version control purposes. Provides a record of the previous versions and iterations of the project. 
/// </summary>
public class LargeItem : MonoBehaviour
{
    public bool inSlot = false;
    public Vector3 slotRotation;
    public Slot currentSlot = null;
    public bool isHeld = false;
    public InputActionProperty rightSelect;
    public float targetTime = 1.0f;
    public GameObject parent;

    /// <summary>
    /// Upon colliding with player hand, removes object from inventory slot
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RightHand")
        {
            if (rightSelect.action.ReadValue<float>() > 0.1f) 
            {
                isHeld = true;
                targetTime = 2.0f;
                if (currentSlot != null)
                {
                    currentSlot.RemoveItem();
                    gameObject.transform.SetParent(parent.transform, false);
                    Vector3 scale = gameObject.transform.localScale;

                    scale.Set(0.5f, 0.5f, 0.5f);

                    gameObject.transform.localScale = scale;
                    gameObject.transform.position = other.gameObject.transform.position;
                    inSlot = false;
                    currentSlot = null;
                    gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                }

            }
        }

    }

    private void Update()
    {
        if (targetTime > 0 && isHeld)
        { 
            targetTime -= Time.deltaTime; 
        }

        if (rightSelect.action.ReadValue<float>() == 0 && targetTime <= 0)
        {
            isHeld = false;
        }
    }
}
