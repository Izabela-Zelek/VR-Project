using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GateAnimStates
{
    Open = 0,
    Closed = 1,
}   
public class ShopGateController : MonoBehaviour
{
    public Animator _animator;
    public static readonly int State = Animator.StringToHash("State");
    public GateAnimStates animState;
    // Start is called before the first frame update
    void Start()
    {
        animState = GateAnimStates.Closed;
        _animator.SetInteger(State, (int)animState);

    }

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
