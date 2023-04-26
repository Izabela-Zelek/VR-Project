using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Old unused script - left in for archival and version control purposes. Provides a record of the previous versions and iterations of the project. 
/// </summary>
public class LargeItem : MonoBehaviour
{
    public bool InSlot = false;
    public Vector3 SlotRotation;
    public Slot CurrentSlot = null;
    public bool IsHeld = false;
    public InputActionProperty RightSelect;
    public float TargetTime = 1.0f;
    public GameObject Parent;

    /// <summary>
    /// Upon colliding with player hand, removes object from inventory slot
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RightHand")
        {
            if (RightSelect.action.ReadValue<float>() > 0.1f) 
            {
                IsHeld = true;
                TargetTime = 2.0f;
                if (CurrentSlot != null)
                {
                    CurrentSlot.RemoveItem();
                    gameObject.transform.SetParent(Parent.transform, false);
                    Vector3 scale = gameObject.transform.localScale;

                    scale.Set(0.5f, 0.5f, 0.5f);

                    gameObject.transform.localScale = scale;
                    gameObject.transform.position = other.gameObject.transform.position;
                    InSlot = false;
                    CurrentSlot = null;
                    gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                }

            }
        }

    }

    private void Update()
    {
        if (TargetTime > 0 && IsHeld)
        { 
            TargetTime -= Time.deltaTime; 
        }

        if (RightSelect.action.ReadValue<float>() == 0 && TargetTime <= 0)
        {
            IsHeld = false;
        }
    }
}
