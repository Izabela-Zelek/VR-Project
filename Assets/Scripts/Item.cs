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
    public bool InSlot = false;
    public Vector3 SlotRotation = Vector3.zero;
    public Slot CurrentSlot = null;
    public bool IsHeld = false;
    public InputActionProperty RightSelect;
    public float TargetTime = 1.0f;
    public bool IsLarge = false;

    private GameObject _parent;
    private XRDirectInteractor _rightInteractor;
    private XRDirectInteractor _leftInteractor;
    private Vector3 _originalScale;

    /// <summary>
    /// Finds required gameobjects in scene
    /// Saves the local scale
    /// </summary>
    private void Start()
    {
        _rightInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(2).GetComponent<XRDirectInteractor>();
        _leftInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(1).GetComponent<XRDirectInteractor>();
        _parent = GameObject.Find("SmallStuff");
        _originalScale = transform.localScale;
    }

    /// <summary>
    /// Upon colliding with player hand, removes object from inventory slot
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!IsLarge)
        {
            if (other.tag == "RightHand")
            {
                if ((_rightInteractor.interactablesSelected.Count > 0 && _rightInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()) || (_leftInteractor.interactablesSelected.Count > 0 && _leftInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()))
                {
                    if (RightSelect.action.ReadValue<float>() > 0.1f)
                    {
                        IsHeld = true;
                        TargetTime = 2.0f;
                        RemoveSmall();

                    }
                }
            }
        }
        else
        {
            if (other.tag == "RightHand")
            {
                if (RightSelect.action.ReadValue<float>() > 0.1f)
                {
                    if ((_rightInteractor.interactablesSelected.Count > 0 && _rightInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()) || (_leftInteractor.interactablesSelected.Count > 0 && _leftInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()))
                    {
                        IsHeld = true;
                        TargetTime = 2.0f;
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
        if (TargetTime > 0 && IsHeld)
        { 
            TargetTime -= Time.deltaTime; 
        }

        if (RightSelect.action.ReadValue<float>() == 0 && TargetTime <= 0)
        {
            IsHeld = false;
        }

        if(!InSlot && gameObject.transform.localScale != _originalScale)
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
        if (CurrentSlot != null)
        {
            CurrentSlot.RemoveItem();
            CurrentSlot.SetPutIn(false);
            gameObject.transform.SetParent(null);

            gameObject.transform.localScale = _originalScale;
            InSlot = false;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            if(GetComponent<MeshCollider>())
            {
                gameObject.GetComponent<MeshCollider>().isTrigger = false;

            }
            else
            {
                gameObject.GetComponent<BoxCollider>().isTrigger = false;

            }
            StartCoroutine(WaitForItem(CurrentSlot));
            CurrentSlot = null;

        }
    }
    /// <summary>
    /// In called to remove small object from inventory, sets parent to null, removes constraints, changes trigger to false
    /// Starts coroutine to temporarily disable the slot from being used
    /// </summary>
    public void RemoveSmall()
    {
        if (CurrentSlot != null)
        {
            CurrentSlot.RemoveItem();
            CurrentSlot.SetPutIn(false);
            gameObject.transform.SetParent(null);

            gameObject.transform.localScale = _originalScale;
            InSlot = false;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            if (GetComponent<MeshCollider>())
            {
                gameObject.GetComponent<MeshCollider>().isTrigger = false;

            }
            else
            {
                gameObject.GetComponent<BoxCollider>().isTrigger = false;

            }
            StartCoroutine(WaitForItem(CurrentSlot));
            CurrentSlot = null;
        }
    }

    /// <summary>
    /// Allows the previously used slot to be used again in 2 seconds
    /// </summary>
    /// <param name="_slot"></param>
    /// <returns></returns>
    private IEnumerator WaitForItem(Slot _slot)
    {
        yield return new WaitForSeconds(2);
        _slot.SetPutIn(true);
    }
}
