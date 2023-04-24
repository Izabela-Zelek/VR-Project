using UnityEngine;
/// <summary>
/// Handles the movement of a flock of birds
/// </summary>
public class BirdFormController : MonoBehaviour
{

    Vector3 endPos;

    /// <summary>
    /// Uses passed in position to face direction of movement
    /// </summary>
    /// <param name="pos"></param>
    public void setDirection(Vector3 pos)
    {
        transform.LookAt(pos);
    }

    /// <summary>
    /// Moves flock forward
    /// </summary>
    private void Update()
    {
        transform.Translate(Vector3.forward * 5 * Time.deltaTime);

    }

    /// <summary>
    /// Destroys flock gameobject upon hit with the boundary
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "BirdBoundary")
        {
            Destroy(this.gameObject);
        }
    }
}
