using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the wander steering behavious of NPCs and avoiding objects in front of them
/// </summary>
public class NPCContoller : MonoBehaviour
{
    [SerializeField]
    private float _circleRadius = 500.0f;
    [SerializeField]
    private float _distance = 300.0f;
    [SerializeField]
    private float _wanderWeight = 1.0f;
    [SerializeField]
    private float _maxSpeed = 2.0f;
    [SerializeField]
    private float _maxForce = 100.0f;
    [SerializeField]
    private bool _startPath = false;
    [SerializeField]
    private int _startTime = 8;

    private Animator _animator;
    private Rigidbody _rb;
    private float _yPos;
    private float _angle;
    private bool _avoid = false;

   /// <summary>
   /// Generates a random angle for rotation
   /// Generates a random wake up time
   /// </summary>
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        _yPos = transform.localPosition.y;
        _animator = GetComponent<Animator>();
        _startTime = Random.Range(7, 13);
    }

    /// <summary>
    /// Checks for the world time to equal their starting time, then enables the NPC and starts movement
    /// 
    /// </summary>
    private void FixedUpdate()
    {
        if (!_startPath)
        {
            if (_startTime == GameObject.Find("GameManager").GetComponent<TimeController>().CurrentTime.Hour)
            {
                _startPath = true;
                if (_animator.runtimeAnimatorController.name != "BasicMotions@Walk")
                {
                    _animator.runtimeAnimatorController = Resources.Load("BasicMotions@Walk") as RuntimeAnimatorController;
                }
            }
        }

        if (_startPath)
        {
            if (!_avoid)
            {
                _rb.AddForce(Wander() * _wanderWeight);

                if (_rb.velocity != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(_rb.velocity, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
                    transform.localPosition = new Vector3(transform.localPosition.x, _yPos, transform.localPosition.z);
                }
            }
        }
    }
    /// <summary>
    /// Moves the NPC by adding a randomly generated small angle value to the rotation of the character and adding force 
    /// </summary>
    /// <returns></returns>
    private Vector3 Wander()
    {
        _angle = _angle + Random.Range(-20, 20) * Mathf.Deg2Rad;

        Vector3 futurePos = transform.position + _rb.velocity * _distance;
        Vector3 pointOnCircle = futurePos;
        pointOnCircle.x = pointOnCircle.x + (_circleRadius * Mathf.Cos(_angle));
        pointOnCircle.z = pointOnCircle.z + (_circleRadius * Mathf.Sin(_angle));

        Vector3 desVelocity = (pointOnCircle - transform.position).normalized * _maxSpeed;

        Vector3 steer = desVelocity - _rb.velocity;

        if (steer.magnitude > _maxForce)
        {
            steer = steer.normalized * _maxForce;
        }

        return steer;
    }

    /// <summary>
    /// Old Implementation of the wander steering behaviour 
    /// Left in for archival and version control purposes. Provides a record of the previous versions and iterations of the project
    /// </summary>
    private void SimpleWander()
    {
        _angle = _angle + Random.Range(-180, 180) * Mathf.Deg2Rad;
    }

    public void Avoid(bool shouldAvoid)
    {
        _avoid = shouldAvoid;
    }

    /// <summary>
    /// Adds the passed in force to allow the NPC to avoid the object in front of them
    /// </summary>
    /// <param name="force"></param>
    public void Avoid(Vector3 force)
    {
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
        }
        _rb.AddForce(force);
    }

    public int GetStartTime()
    {
        return _startTime;
    }
}
