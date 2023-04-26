using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Stores the two possible animation states
/// </summary>
public enum GateAnimStates
{
    Open = 0,
    Closed = 1,
}
/// <summary>
/// Handles the animations of the shopping stalls opening and closing
/// </summary>
public class ShopGateController : MonoBehaviour
{
    public Animator _animator;
    public static readonly int State = Animator.StringToHash("State");
    public GateAnimStates animState;

    void Start()
    {
        animState = GateAnimStates.Closed;
        _animator.SetInteger(State, (int)animState);

    }
    /// <summary>
    /// Grabs the current time of day
    /// If time == 7am, plays the opening stall animation
    /// If time == 5pm, plays the closing stall animation
    /// </summary>
    private void Update()
    {
        if (GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.ToString("HH:mm") == "17:00")
        {
            CloseShop();
            GameObject.Find("GameManager").GetComponent<GameManager>().shopOpen = false;
        }
        if (GameObject.Find("GameManager").GetComponent<TimeController>().currentTime.ToString("HH:mm") == "07:00")
        {
            OpenShop();
            GameObject.Find("GameManager").GetComponent<GameManager>().shopOpen = true;
        }
    }
    public void CloseShop()
    {
        animState = GateAnimStates.Closed;
        _animator.SetInteger(State, (int)animState);
    }

    public void OpenShop()
    {
        animState = GateAnimStates.Open;
        _animator.SetInteger(State, (int)animState);
    }
}
