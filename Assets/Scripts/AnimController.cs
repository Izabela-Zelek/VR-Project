using UnityEngine;
/// <summary>
/// Defines two possible animation states
/// </summary>
public enum TiredAnimStates
{
    Woke = 0,
    Slep = 1,
}
/// <summary>
/// Handles the eye animations for falling asleep and waking up
/// </summary>
public class AnimController : MonoBehaviour
{
    public Animator Animator;
    public static readonly int State = Animator.StringToHash("State");
    public TiredAnimStates AnimState;

    /// <summary>
    /// Sets initial animation state to waking up
    /// </summary>
    void Start()
    {
        AnimState = TiredAnimStates.Woke;
        Animator.SetInteger(State, (int)AnimState);

    }

    /// <summary>
    /// Sets animation state to falling asleep
    /// </summary>
    public void getTired()
    {
        AnimState = TiredAnimStates.Slep;
        Animator.SetInteger(State, (int)AnimState);
    }
    /// <summary>
    /// Sets animation state to waking up
    /// </summary>
    public void awaken()
    {
        AnimState = TiredAnimStates.Woke;
        Animator.SetInteger(State, (int)AnimState);
    }
}
