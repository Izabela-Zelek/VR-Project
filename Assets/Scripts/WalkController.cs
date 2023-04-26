using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
/// <summary>
/// Handles the walking audio
/// </summary>
public class WalkController : MonoBehaviour
{
    private AudioSource _walk;
    void Start()
    {
        _walk = transform.Find("Feet").GetComponent<AudioSource>();
    }
    /// <summary>
    /// If taking in walking input, starts looping through walk audio
    /// If no input, stops walking audio
    /// </summary>
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
