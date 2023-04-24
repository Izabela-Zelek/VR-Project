using UnityEngine;

/// <summary>
/// Handles the behaviour of birds upon being hit by bullet
/// </summary>
public class BirdController : MonoBehaviour
{
    private GameObject _feather;

    /// <summary>
    /// Loads feather prefab from Resources folder upon start
    /// </summary>
    private void Start()
    {
        _feather = Resources.Load("SNature_Feather") as GameObject;
    }
    /// <summary>
    /// Upon collision with bullet, spawns a feather object in its position and destroys the bird gameobject
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bullet")
        {
            int randNum = Random.Range(1, 3);

            for(int i = 0; i < randNum; i++)
            {
                Instantiate(_feather,transform.position,Quaternion.identity);
            }
            Destroy(this.gameObject);
        }
    }
}
