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
    public Animator _animator;
    public static readonly int State = Animator.StringToHash("State");
    public TiredAnimStates animState;
    /// <summary>
    /// Sets initial animation state to waking up
    /// </summary>
    void Start()
    {
        animState = TiredAnimStates.Woke;
        _animator.SetInteger(State, (int)animState);

    }

    /// <summary>
    /// Sets animation state to falling asleep
    /// </summary>
    public void getTired()
    {
        animState = TiredAnimStates.Slep;
        _animator.SetInteger(State, (int)animState);
    }
    /// <summary>
    /// Sets animation state to waking up
    /// </summary>
    public void awaken()
    {
        animState = TiredAnimStates.Woke;
        _animator.SetInteger(State, (int)animState);
    }
}
