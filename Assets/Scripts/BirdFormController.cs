using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdFormController : MonoBehaviour
{

    Vector3 endPos;

    public void setDirection(Vector3 pos)
    {
        transform.LookAt(pos);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * 5 * Time.deltaTime);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "BirdBoundary")
        {
            Destroy(this.gameObject);
        }
    }
}
