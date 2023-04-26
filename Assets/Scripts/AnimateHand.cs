using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Handles hand animations upon press of controller buttons to add immersion
/// </summary>
public class AnimateHand : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;

    public Animator handAnimator;

    /// <summary>
    /// Changes animation state of hand model depending on input values
    /// </summary>
    void Update()
    {
       float triggerValue = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = gripAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);
    }
}
