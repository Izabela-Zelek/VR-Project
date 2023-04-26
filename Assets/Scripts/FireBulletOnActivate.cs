//Script taken from Unity XR tutorial by Valem Tutorials
//https://www.youtube.com/watch?v=0xt6dACM_1I&list=PLpEoiloH-4eP-OKItF8XNJ8y8e1asOJud&index=6&t=426s&ab_channel=ValemTutorials
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FireBulletOnActivate : MonoBehaviour
{
    public GameObject Bullet;
    public Transform SpawnPoint;
    public float FireSpeed = 20;
    private AudioSource _shoot;
    // Start is called before the first frame update
    private void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireBullet);
        _shoot = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void FireBullet(ActivateEventArgs arg)
    {
        _shoot.Play();
        GameObject spawnedBullet = Instantiate(Bullet);
        spawnedBullet.transform.position = SpawnPoint.position;
        spawnedBullet.GetComponent<Rigidbody>().velocity = SpawnPoint.forward * FireSpeed;
        Destroy(spawnedBullet, 15);
    }
}
