using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
/// <summary>
/// Handles determining whether current object is being held or in inventory slot
/// </summary>
public class Item : MonoBehaviour
{
    public bool inSlot = false;
    public Vector3 slotRotation = Vector3.zero;
    public Slot currentSlot = null;
    public bool isHeld = false;
    public InputActionProperty rightSelect;
    public float targetTime = 1.0f;
    private GameObject parent;
    public bool isLarge = false;
    private XRDirectInteractor rightInteractor;
    private XRDirectInteractor leftInteractor;
    private Vector3 _originalScale;

    /// <summary>
    /// Finds required gameobjects in scene
    /// Saves the local scale
    /// </summary>
    private void Start()
    {
        rightInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(2).GetComponent<XRDirectInteractor>();
        leftInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(1).GetComponent<XRDirectInteractor>();
        parent = GameObject.Find("SmallStuff");
        _originalScale = transform.localScale;
    }

    /// <summary>
    /// Upon colliding with player hand, removes object from inventory slot
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!isLarge)
        {
            if (other.tag == "RightHand")
            {
                if ((rightInteractor.interactablesSelected.Count > 0 && rightInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()) || (leftInteractor.interactablesSelected.Count > 0 && leftInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()))
                {
                    if (rightSelect.action.ReadValue<float>() > 0.1f)
                    {
                        isHeld = true;
                        targetTime = 2.0f;
                        RemoveSmall();

                    }
                }
            }
        }
        else
        {
            if (other.tag == "RightHand")
            {
                if (rightSelect.action.ReadValue<float>() > 0.1f)
                {
                    if ((rightInteractor.interactablesSelected.Count > 0 && rightInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()) || (leftInteractor.interactablesSelected.Count > 0 && leftInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()))
                    {
                        isHeld = true;
                        targetTime = 2.0f;
                        Remove();
                    }
                }

            }
        }

    }

    /// <summary>
    /// Changes scale of object to original scale in not in slot
    /// </summary>
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

        if(!inSlot && gameObject.transform.localScale != _originalScale)
        {
            gameObject.transform.localScale = _originalScale;
        }
    }

    /// <summary>
    /// In called to remove object from inventory, sets parent to null, removes constraints, changes trigger to false
    /// Starts coroutine to temporarily disable the slot from being used
    /// </summary>
    public void Remove()
    {
        if (currentSlot != null)
        {
            currentSlot.RemoveItem();
            currentSlot.SetPutIn(false);
            gameObject.transform.SetParent(null);

            gameObject.transform.localScale = _originalScale;
            inSlot = false;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            if(GetComponent<MeshCollider>())
            {
                gameObject.GetComponent<MeshCollider>().isTrigger = false;

            }
            else
            {
                gameObject.GetComponent<BoxCollider>().isTrigger = false;

            }
            StartCoroutine(WaitForItem(currentSlot));
            currentSlot = null;

        }
    }
    /// <summary>
    /// In called to remove small object from inventory, sets parent to null, removes constraints, changes trigger to false
    /// Starts coroutine to temporarily disable the slot from being used
    /// </summary>
    public void RemoveSmall()
    {
        if (currentSlot != null)
        {
            currentSlot.RemoveItem();
            currentSlot.SetPutIn(false);
            gameObject.transform.SetParent(null);

            gameObject.transform.localScale = _originalScale;
            inSlot = false;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            if (GetComponent<MeshCollider>())
            {
                gameObject.GetComponent<MeshCollider>().isTrigger = false;

            }
            else
            {
                gameObject.GetComponent<BoxCollider>().isTrigger = false;

            }
            StartCoroutine(WaitForItem(currentSlot));
            currentSlot = null;
        }
    }

    /// <summary>
    /// Allows the previously used slot to be used again in 2 seconds
    /// </summary>
    /// <param name="_slot"></param>
    /// <returns></returns>
    IEnumerator WaitForItem(Slot _slot)
    {
        yield return new WaitForSeconds(2);
        _slot.SetPutIn(true);
    }
}
