using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Old implementation of the Object Avoidance steering behaviour - left in for archival and version control purposes. Provides a record of the previous versions and iterations of the project
/// </summary>
public class ObjectAvoid : MonoBehaviour
{
    private float _raycastDistance = 2;
    private float newAngle;
    /// <summary>
    /// Casts a raycast in front of the NPC and if it hits any object that isn't the ground, tells the NPC to avoid
    /// </summary>
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
            newAngle = 0;
        }
    }
}
