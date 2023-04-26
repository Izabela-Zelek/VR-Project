using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Handles enabling or disabling inventory upon button press
/// </summary>
public class InventoryVR : MonoBehaviour
{
    public InputActionProperty LeftActivate;
    public GameObject Inventory;
    public GameObject Anchor;

    private bool _UIActive;

    /// <summary>
    /// Sets initial state of inventory to inactive
    /// </summary>
    private void Start()
    {
        Inventory.SetActive(false);
        _UIActive = false;

    }

    /// <summary>
    /// Upon button press, enables inventory
    /// Sets position of inventory to in front of player
    /// Upon button release, disables inventory
    /// </summary>
    private void Update()
    {
       
        if (LeftActivate.action.ReadValue<float>() > 0.1f)
        {
            _UIActive = true;
            Inventory.SetActive(_UIActive);
        }
        else
        {
            _UIActive = false;
            Inventory.SetActive(_UIActive);
        }
        if (_UIActive)
        {
            Inventory.transform.position = Anchor.transform.position;
            Inventory.transform.eulerAngles = new Vector3(Anchor.transform.eulerAngles.x + 15, Anchor.transform.eulerAngles.y, 0);
        }
    }
}