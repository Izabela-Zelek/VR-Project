using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class WalkController : MonoBehaviour
{
    //public XRNode _leftInputSource;
    //public XRNode _rightInputSource;
    private AudioSource _walk;
    void Start()
    {
        _walk = transform.Find("Feet").GetComponent<AudioSource>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            if (!_walk.isPlaying)
            {
                _walk.Play();
            }
        }
        else
        {
            if (_walk.isPlaying)
            {
                _walk.Stop();
            }
        }
    }

}
