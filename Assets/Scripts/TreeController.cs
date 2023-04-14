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

    private void Start()
    {
        _log = Resources.Load("Firewood") as GameObject;
        _treeFall = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(_falling && !_treeFall.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
    public void MinusLife()
    {
        life--;
        if (life == 0)
        {
            for(int i = 0; i < woodCount;i++)
            {
                Instantiate(_log, transform.Find("Spawn").position, Quaternion.identity);
            }
            _treeFall.Play();
            _falling = true;
        }
    }
}
