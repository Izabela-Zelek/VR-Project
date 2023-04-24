using UnityEngine;
/// <summary>
/// Handles initial spawn collisions of grass
/// </summary>
public class GrassCollider : MonoBehaviour
{
    private float _startUpTime = 10.0f;
    private bool _checked = false;

    /// <summary>
    /// Upon collision with collider gameobject 
    /// If gameobject isn't the ground, destroys grass gameobject
    /// </summary>
    /// <param name="collision"></param>
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
    /// <summary>
    /// Upon collision with trigger gameobject 
    /// If gameobject isn't the ground, destroys grass gameobject
    /// </summary>
    /// <param name="other"></param>
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
