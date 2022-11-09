using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlantState
{
    Bare = 0,
    Seed = 1,
    Growing = 2,
    Grown = 3,
    Fruit = 4
}
public class FarmScript : MonoBehaviour
{
    public GameObject seeds;
    public PlantState plantState;

    private void Start()
    {
        plantState = PlantState.Bare;
    }
    public void plantSeeds()
    {
        if (plantState == PlantState.Bare)
        {
            Vector3 pos = new Vector3(transform.position.x, 0.02625f, transform.position.z);
            Instantiate(seeds, pos, Quaternion.identity, gameObject.transform);
            plantState = PlantState.Seed;
        }
    }

}
