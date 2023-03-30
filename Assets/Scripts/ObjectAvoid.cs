using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAvoid : MonoBehaviour
{
    private float _raycastDistance = 2;
    private float newAngle;
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position,transform.forward,out hit, _raycastDistance))
        {
            if (hit.collider.tag != "Ground" && hit.collider.tag != "Plane")
            {
                Debug.Log(hit.collider.name);
                Vector3 hitNormal = hit.normal;
                Vector3 hitDirection = Vector3.Reflect(transform.forward, hitNormal);
                float angle = Vector3.Angle(transform.forward, hitDirection);

                newAngle = angle;
            }
            GetComponent<NPCContoller>().Avoid(true);         
            transform.Rotate(0, newAngle, 0);
        }
        else
        {
            GetComponent<NPCContoller>().Avoid(false);
            //transform.Translate(Vector3.forward * 1 * Time.deltaTime);
            newAngle = 0;
        }
    }
}
