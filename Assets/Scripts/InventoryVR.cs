using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Handles enabling or disabling inventory upon button press
/// </summary>
public class InventoryVR : MonoBehaviour
{
    public InputActionProperty leftActivate;

    public GameObject Inventory;
    public GameObject Anchor;
    bool UIActive;

    /// <summary>
    /// Sets initial state of inventory to inactive
    /// </summary>
    private void Start()
    {
        Inventory.SetActive(false);
        UIActive = false;

    }

    /// <summary>
    /// Upon button press, enables inventory
    /// Sets position of inventory to in front of player
    /// Upon button release, disables inventory
    /// </summary>
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