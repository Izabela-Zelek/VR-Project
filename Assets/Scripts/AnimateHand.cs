using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Handles hand animations upon press of controller buttons to add immersion
/// </summary>
public class AnimateHand : MonoBehaviour
{
    public InputActionProperty PinchAnimationAction;
    public InputActionProperty GripAnimationAction;
    public Animator HandAnimator;

    /// <summary>
    /// Changes animation state of hand model depending on input values
    /// </summary>
    private void Update()
    {
       float triggerValue = PinchAnimationAction.action.ReadValue<float>();
        HandAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = GripAnimationAction.action.ReadValue<float>();
        HandAnimator.SetFloat("Grip", gripValue);
    }
}
