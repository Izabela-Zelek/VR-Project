using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
/// <summary>
/// Handles enabling or disabling the teleportation ray upon button press
/// </summary>
public class ActivateTeleportationRay : MonoBehaviour
{
    public GameObject rightTeleportation;

    public InputActionProperty rightActivate;

    public InputActionProperty rightCancel;
    void Update()
    {
        rightTeleportation.SetActive(rightCancel.action.ReadValue<float>() == 0 && rightActivate.action.ReadValue<float>() > 0.1f);
    }
}
