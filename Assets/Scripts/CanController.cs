using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanController : MonoBehaviour
{
    public InputActionProperty rightSelect;
    //private void Update()
    //{
    //    Debug.DrawRay(transform.GetChild(1).transform.position, transform.up + transform.forward, Color.black, 1);
    //}

    private void OnTriggerEnter(Collider other)
    {
        //RaycastHit seen;
        //Ray raydirection = new Ray(transform.GetChild(1).transform.position, transform.up + transform.forward );
        //if (Physics.Raycast(raydirection, out seen, 5))
        //{
            if (other.tag == "SeedArea" && rightSelect.action.ReadValue<float>() >= 0.1f)
            {
                transform.GetChild(1).transform.GetChild(0).GetComponent<RainController>().turnOnWater();
            }
            //else
            //{
            //    transform.GetChild(1).transform.GetChild(0).GetComponent<RainController>().turnOffWater();
            //}
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SeedArea")
        {
            transform.GetChild(1).transform.GetChild(0).GetComponent<RainController>().turnOffWater();
        }
    }
}
