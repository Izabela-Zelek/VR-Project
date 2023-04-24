using UnityEngine;
/// <summary>
/// Handles the functionality of the axe object
/// </summary>
public class AxeController : MonoBehaviour
{
    private bool colliding = false;
    private AudioSource _chop;

    private void Start()
    {
        _chop = GetComponent<AudioSource>();
    }
    /// <summary>
    /// Casts raycast to detect collisions with Tree objects
    /// Upon collision, takes 1 life away from the tree and plays chopping audio
    /// </summary>
    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up, out hit, 0.1f))
        {
            if (hit.collider.tag == "Tree" && !colliding)
            {
                GameObject tree = hit.collider.gameObject;
                while (tree.transform.parent.tag != "Parent")
                {
                    tree = tree.transform.parent.gameObject;
                }
               
                tree.GetComponent<TreeController>().MinusLife(hit.point);
                _chop.Play();
                colliding = true;
            }
        }
        else
        {
            colliding = false;
        }
    }
}
