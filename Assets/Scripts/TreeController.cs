using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the lifespan of a tree. Keeps track of the life count and destroys the tree and instantiates a random nr of logs once it lost all lives
/// </summary>
public class TreeController : MonoBehaviour
{
    public int WoodCount;

    private int _life = 3;
    private GameObject _log;
    private AudioSource _treeFall;
    private bool _falling = false;
    private Quaternion _oppRot;

    private void Start()
    {
        _log = Resources.Load("Firewood") as GameObject;
        _treeFall = GetComponent<AudioSource>();
    }

    /// <summary>
    /// If the tree is set to falling, rotates the tree to fall in opposite direction of hit
    /// Destroys tree once it reaches the opposite rotation
    /// </summary>
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
    /// <summary>
    /// Takes away one life of the tree 
    /// If no more lives left, spawns logs in its place that the player can pick up
    /// </summary>
    /// <param name="hitPoint"></param>
    public void MinusLife(Vector3 hitPoint)
    {
        _life--;
        if (_life == 0)
        {
            for(int i = 0; i < WoodCount;i++)
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
