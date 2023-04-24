using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

/// <summary>
/// Handles behaviour of fruit when harvested
/// </summary>
public class FruitController : MonoBehaviour
{
    [SerializeField]
    private GameObject stem;
    private Transform parent;
    private bool pickedUp = false;
    private XRDirectInteractor rightInteractor;
    private XRDirectInteractor leftInteractor;
    [SerializeField]
    private InputActionProperty rightSelect;
    public int price;


    private void Start()
    {
        rightInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(2).GetComponent<XRDirectInteractor>();
        leftInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(1).GetComponent<XRDirectInteractor>();
        parent = GameObject.Find("SmallStuff").transform;

    }
   /// <summary>
   /// if fruit grew on bundle and interacts with player hand, decrements number of children of parent stem
   /// If stem is now at 0 children, destroys stem gameobject
   /// Removes constraints of fruit which stopped its reaction to gravity
   /// </summary>
    void Update()
    {
        if(stem != null && !pickedUp)
        {
            if ((rightInteractor.interactablesSelected.Count > 0 && rightInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()) || (leftInteractor.interactablesSelected.Count > 0 && leftInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()))
            {
                if (rightSelect.action.ReadValue<float>() >= 0.1f)
                {
                    stem.GetComponent<MultiFruitStemController>().MinusChild();
                    if (stem.GetComponent<MultiFruitStemController>().childCount == 0)
                    { 
                        Destroy(stem.gameObject); 
                    }
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    pickedUp = true;
                }
            }
        }
        if(pickedUp)
        {
            gameObject.transform.SetParent(null);
            gameObject.transform.SetParent(parent);
        }
    }
}
