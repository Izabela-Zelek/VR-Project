using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    private GameObject _feather;
    private void Start()
    {
        _feather = Resources.Load("SNature_Feather") as GameObject;
    }
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
