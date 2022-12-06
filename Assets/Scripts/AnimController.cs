using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TiredAnimStates
{
    Woke = 0,
    Slep = 1,
}
public class AnimController : MonoBehaviour
{
    public Animator _animator;
    public static readonly int State = Animator.StringToHash("State");
    public TiredAnimStates animState;
    // Start is called before the first frame update
    void Start()
    {
        animState = TiredAnimStates.Woke;
        _animator.SetInteger(State, (int)animState);

    }

    public void getTired()
    {
        animState = TiredAnimStates.Slep;
        _animator.SetInteger(State, (int)animState);
    }

    public void awaken()
    {
        animState = TiredAnimStates.Woke;
        _animator.SetInteger(State, (int)animState);
    }
}
