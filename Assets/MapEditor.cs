using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class MapEditor : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    public Transform centre;
    public Transform left;
    public Transform camCentre;
    public Transform camLeft;

    public InputActionProperty rightSelect;

    private float mapDistance;
    private float camDistance;
    private float multiplier;

    bool clicked = false;

    private void Start()
    {
        mapDistance = Vector3.Distance(centre.position, left.position);
        camDistance = Vector3.Distance(camCentre.position, camLeft.position);

        multiplier = camDistance / mapDistance;
    }
    private
    void Update()
    {

        if (rightSelect.action.ReadValue<float>() >= 0.1f)
        {
            RaycastHit res;
            if (rayInteractor.TryGetCurrent3DRaycastHit(out res))
            {
                Vector3 hitPoint = res.point; // the coordinate that the ray hits

                if (res.collider.name == "Map" && !clicked)
                {
                    clicked = true;
                    float yDistance = Mathf.Sqrt((hitPoint.y - centre.position.y) * (hitPoint.y - centre.position.y));
                    float zDistance = Mathf.Sqrt((hitPoint.z - centre.position.z) * (hitPoint.z - centre.position.z));

                    if (hitPoint.z > centre.position.z)
                    {
                        zDistance = -zDistance;
                    }
                    zDistance = zDistance * multiplier;

                    if (hitPoint.y > centre.position.y)
                    {
                        yDistance = -yDistance;
                    }
                    yDistance = yDistance * multiplier;

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(camCentre.position.x + yDistance, 0.5f, camCentre.position.z + zDistance);

                }
            }
        }
        else if (rightSelect.action.ReadValue<float>() <= 0.0f)
        {
            clicked = false;
        }
    }
}
