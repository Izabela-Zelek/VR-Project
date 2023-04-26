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
    public GameObject RightTeleportation;

    public InputActionProperty RightActivate;

    public InputActionProperty RightCancel;
    void Update()
    {
        RightTeleportation.SetActive(RightCancel.action.ReadValue<float>() == 0 && RightActivate.action.ReadValue<float>() > 0.1f);
    }
}
