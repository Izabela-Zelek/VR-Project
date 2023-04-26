using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

/// <summary>
/// Handles behaviour of fruit when harvested
/// </summary>
public class FruitController : MonoBehaviour
{
    public int Price;

    [SerializeField]
    private GameObject _stem;
    [SerializeField]
    private InputActionProperty _rightSelect;

    private Transform _parent;
    private bool _pickedUp = false;
    private XRDirectInteractor _rightInteractor;
    private XRDirectInteractor _leftInteractor;


    private void Start()
    {
        _rightInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(2).GetComponent<XRDirectInteractor>();
        _leftInteractor = GameObject.Find("XR Origin").transform.GetChild(0).transform.GetChild(1).GetComponent<XRDirectInteractor>();
        _parent = GameObject.Find("SmallStuff").transform;

    }
   /// <summary>
   /// if fruit grew on bundle and interacts with player hand, decrements number of children of parent stem
   /// If stem is now at 0 children, destroys stem gameobject
   /// Removes constraints of fruit which stopped its reaction to gravity
   /// </summary>
    private void Update()
    {
        if(_stem != null && !_pickedUp)
        {
            if ((_rightInteractor.interactablesSelected.Count > 0 && _rightInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()) || (_leftInteractor.interactablesSelected.Count > 0 && _leftInteractor.interactablesSelected[0] == this.GetComponent<IXRSelectInteractable>()))
            {
                if (_rightSelect.action.ReadValue<float>() >= 0.1f)
                {
                    _stem.GetComponent<MultiFruitStemController>().MinusChild();
                    if (_stem.GetComponent<MultiFruitStemController>().ChildCount == 0)
                    { 
                        Destroy(_stem.gameObject); 
                    }
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    _pickedUp = true;
                }
            }
        }
        if(_pickedUp)
        {
            gameObject.transform.SetParent(null);
            gameObject.transform.SetParent(_parent);
        }
    }
}
