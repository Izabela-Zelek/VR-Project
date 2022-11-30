using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

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
    private XRDirectInteractor rightInteractor = new XRDirectInteractor();
    private XRDirectInteractor leftInteractor = new XRDirectInteractor();

    private void Start()
    {
        rightInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(2).GetComponent<XRDirectInteractor>();
        leftInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(1).GetComponent<XRDirectInteractor>();
        parent = GameObject.Find("SmallStuff");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLarge)
        {
            if (other.tag == "RightHand")
            {
                if ( (rightInteractor.interactablesSelected.Count > 0 && rightInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()) ||( leftInteractor.interactablesSelected.Count > 0 && leftInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()))
                {
                    if (rightSelect.action.ReadValue<float>() > 0.1f)
                    {
                        isHeld = true;
                        targetTime = 2.0f;
                        if (currentSlot != null)
                        {
                            currentSlot.RemoveItem();
                            gameObject.transform.SetParent(parent.transform);
                            transform.position = other.gameObject.transform.position;
                            inSlot = false;
                            currentSlot = null;
                            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                            gameObject.GetComponent<BoxCollider>().isTrigger = false;

                        }

                    }
                }
            }
        }
        else
        {
            if (other.tag == "RightHand")
            {
                if ((rightInteractor.interactablesSelected.Count > 0 && rightInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()) || (leftInteractor.interactablesSelected.Count > 0 && leftInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()))
                {
                    if (rightSelect.action.ReadValue<float>() > 0.1f)
                    {
                        isHeld = true;
                        targetTime = 2.0f;
                        if (currentSlot != null)
                        {
                            currentSlot.RemoveItem();
                            gameObject.transform.SetParent(parent.transform);
                            Vector3 scale = gameObject.transform.localScale;

                            scale.Set(0.5f, 0.5f, 0.5f);

                            gameObject.transform.localScale = scale;
                            gameObject.transform.position = other.gameObject.transform.position;
                            inSlot = false;
                            currentSlot = null;
                            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                            gameObject.GetComponent<MeshCollider>().isTrigger = false;


                        }

                    }
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

        if ((rightInteractor.interactablesSelected.Count > 0 && rightInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()) || (leftInteractor.interactablesSelected.Count > 0 && leftInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()))
        {
            isHeld = true;
            targetTime = 2.0f;
            if (currentSlot != null)
            {
                currentSlot.RemoveItem();
                gameObject.transform.SetParent(parent.transform);
                inSlot = false;
                currentSlot = null;
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                gameObject.GetComponent<BoxCollider>().isTrigger = false;

            }
        }
    }
}
