using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassCollider : MonoBehaviour
{
    private float _startUpTime = 10.0f;
    private bool _checked = false;
    private void OnCollisionStay(Collision collision)
    {
        if (!_checked)
        {
            if (collision.collider.tag != "Plane" && collision.collider.tag != "Ground")
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_checked)
        {
            if (other.tag != "Plane" && other.tag != "Ground")
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void Update()
    {
        if(!_checked)
        {
            _startUpTime -= Time.deltaTime;
        }

        if(_startUpTime <= 0)
        {
            _checked = true;
        }
    }
}
