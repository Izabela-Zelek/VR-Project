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
    public InputActionProperty rotate;

    public MeshRenderer option1;
    public MeshRenderer option2;
    public MeshRenderer option3;

    private float mapDistance;
    private float camDistance;
    private float multiplier;

    private int chosenOption = 0;

    private bool clicked = false;
    private bool rotating = false;

    private GameObject tree;
    private GameObject tree2;
    private GameObject house;

    private Material notChosenMat;
    private Material chosenMat;

    LineRenderer lineRenderer = new LineRenderer();
    Vector3[] positions = new Vector3[3];

    private List<GameObject> followObjects = new List<GameObject>();
    private void Start()
    {
        mapDistance = Vector3.Distance(centre.position, left.position);
        camDistance = Vector3.Distance(camCentre.position, camLeft.position);

        multiplier = camDistance / mapDistance;

        tree = Resources.Load("Tree_1_1") as GameObject;
        tree2 = Resources.Load("Tree_1_2") as GameObject;
        house = Resources.Load("ShopGuyHouse") as GameObject;

        notChosenMat = Resources.Load("notChosen_mat") as Material;
        chosenMat = Resources.Load("chosen_mat") as Material;

    }
    private
    void Update()
    {
        
        if(rotate.action.ReadValue<Vector2>().x == 0)
        {
            Debug.Log("PAUSE");
        }
        else if (rotate.action.ReadValue<Vector2>().x > 0)
        {
            Debug.Log("RIGHT");
        }
        else if (rotate.action.ReadValue<Vector2>().x < 0)
        {
            Debug.Log("LEFT");
        }

        if (rightSelect.action.ReadValue<float>() >= 0.1f)
        {
            RaycastHit res;
            if (rayInteractor.TryGetCurrent3DRaycastHit(out res))
            {
                Vector3 hitPoint = res.point; // the coordinate that the ray hits

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

                Vector3 pos = new Vector3(camCentre.position.x + yDistance, 0.0f, camCentre.position.z + zDistance);

                if (res.collider.name == "Tree1" && !clicked)
                {
                    if (chosenOption != 1)
                    {
                        resetColours();
                        chosenOption = 1;
                        option1.material = chosenMat;
                    }
                    else
                    {
                        resetColours();
                        chosenOption = 0;
                    }
                }
                else if(res.collider.name == "Tree2" && !clicked)
                {
                    if (chosenOption != 2)
                    {
                        resetColours();
                        chosenOption = 2;
                        option2.material = chosenMat;
                    }
                    else
                    {
                        resetColours();
                        chosenOption = 0;
                    }
                }
                else if (res.collider.name == "House" && !clicked)
                {
                    if (chosenOption != 3)
                    {
                        resetColours();
                        chosenOption = 3;
                        option3.material = chosenMat;
                    }
                    else
                    {
                        resetColours();
                        chosenOption = 0;
                    }
                }
                else if (res.collider.name == "Map" && !clicked)
                {
                    clicked = true;

                    switch (chosenOption)
                    {
                        case 0:
                            Collider[] hitColliders = Physics.OverlapSphere(pos, 1.0f);
                            foreach (var hitCollider in hitColliders)
                            {
                                if(hitCollider.tag != "Ground" && hitCollider.tag != "Plane" && hitCollider.tag != "Boundary")
                                {
                                    GameObject hit = hitCollider.gameObject;
                                    while (hit.transform.parent != null)
                                    {
                                        if (hit.transform.parent.tag == "Parent")
                                        {
                                            break;
                                        }
                                        hit = hit.transform.parent.gameObject;
                                    }
                                    Debug.Log(hit.name);
                                    Debug.Log(hit.transform.parent);
                                    followObjects.Add(hit);
                                }
                            }

                            break;
                        case 1:
                            Instantiate(tree, pos, Quaternion.identity);
                            break;
                        case 2:
                            Instantiate(tree2, pos, Quaternion.identity);
                            break;
                        case 3:
                            Instantiate(house, pos, Quaternion.identity);
                            break;
                    }
                   
                   
                }

                foreach (GameObject item in followObjects)
                {
                    item.transform.position = new Vector3(pos.x,item.transform.position.y,pos.z);
                }

                if (!rotating && followObjects.Count > 0)
                {
                    positions[0] = followObjects[0].transform.position;
                    positions[1] = followObjects[0].transform.position + followObjects[0].transform.forward*10;
                }

                if (rotate.action.ReadValue<Vector2>().x > 0)
                {
                    rotating = true;
                    foreach (GameObject item in followObjects)
                    {
                        item.transform.eulerAngles = new Vector3(item.transform.eulerAngles.x, item.transform.eulerAngles.y + 1, item.transform.eulerAngles.z);
                    }
                }
                else if (rotate.action.ReadValue<Vector2>().x < 0)
                {
                    rotating = true;
                    foreach (GameObject item in followObjects)
                    {
                        item.transform.eulerAngles = new Vector3(item.transform.eulerAngles.x, item.transform.eulerAngles.y - 1, item.transform.eulerAngles.z);
                    }

                }
                //if (rotating && followObjects.Count > 0)
                //{
                //    positions[2] = followObjects[0].transform.position + followObjects[0].transform.forward * 10;

                //    lineRenderer.SetPositions(positions);
                //}
            }
           
        }
        else if (rightSelect.action.ReadValue<float>() <= 0.0f)
        {
            clicked = false;
            followObjects.Clear();
            rotating = false;
        }


    }

    void resetColours()
    {
        option1.material = notChosenMat;
        option2.material = notChosenMat;
        option3.material = notChosenMat;
    }
}
