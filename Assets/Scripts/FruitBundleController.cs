using UnityEngine;
/// <summary>
/// Handles destruction of plants with multiple fruit
/// </summary>
public class FruitBundleController : MonoBehaviour
{
    /// <summary>
    /// If all fruit have been collected from bundle, destroys bundle gameobject
    /// </summary>
    private void Update()
    {
        if(gameObject.transform.childCount == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
