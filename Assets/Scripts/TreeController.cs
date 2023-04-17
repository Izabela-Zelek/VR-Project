using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    private int life = 3;
    public int woodCount;
    private GameObject _log;
    private AudioSource _treeFall;
    private bool _falling = false;
    private Quaternion _oppRot;
    private void Start()
    {
        _log = Resources.Load("Firewood") as GameObject;
        _treeFall = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(_falling && !_treeFall.isPlaying)
        {
            //Destroy(this.gameObject);
        }
    }
    private void FixedUpdate()
    {
        if (_falling)
        { 
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, _oppRot, 0.00000001f);
            transform.rotation = rotation;
        }
        if(transform.rotation == _oppRot)
        {
            Destroy(this.gameObject);
        }
    }
    public void MinusLife(Vector3 hitPoint)
    {
        life--;
        if (life == 0)
        {
            for(int i = 0; i < woodCount;i++)
            {
                Instantiate(_log, transform.Find("Spawn").position, Quaternion.identity);
            }
            _treeFall.Play();

            Vector3 hitDir = transform.position - hitPoint;
            Vector3 oppDir = hitDir.normalized;
            _oppRot = Quaternion.LookRotation(oppDir);
            transform.rotation = _oppRot;

            _falling = true;
       
        }
    }
}
