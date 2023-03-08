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
    public MeshRenderer option4;
    public MeshRenderer option5;
    public MeshRenderer option6;
    public MeshRenderer option7;
    public MeshRenderer option8;
    public MeshRenderer option9;

    private float mapDistance;
    private float camDistance;
    private float multiplier;

    private int chosenOption = 0;

    private bool clicked = false;
    private bool rotating = false;

    private GameObject tree;
    private GameObject tree2;
    private GameObject house;
    private GameObject pathNPC;
    private GameObject wanderNPC;

    private Material notChosenMat;
    private Material chosenMat;

    private bool makingPath = false;
    private List<Vector3> customPath = new List<Vector3>();

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
        pathNPC = Resources.Load("PathNPC") as GameObject;
        wanderNPC = Resources.Load("WanderNPC") as GameObject;

        notChosenMat = Resources.Load("notChosen_mat") as Material;
        chosenMat = Resources.Load("chosen_mat") as Material;
    }
    private
    void Update()
    {
        
        //if(rotate.action.ReadValue<Vector2>().x == 0)
        //{
        //    Debug.Log("PAUSE");
        //}
        //else if (rotate.action.ReadValue<Vector2>().x > 0)
        //{
        //    Debug.Log("RIGHT");
        //}
        //else if (rotate.action.ReadValue<Vector2>().x < 0)
        //{
        //    Debug.Log("LEFT");
        //}

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

                if (!clicked)
                {
                    switch (res.collider.name)
                    {
                        case "Tree1":
                            if (chosenOption != 1)
                            {
                                resetColours();
                                chosenOption = 1;
                                option1.material = chosenMat;
                                clicked = true;
                            }
                            else
                            {
                                resetColours();
                                chosenOption = 0;
                                clicked = true;
                            }
                            break;
                        case "Tree2":
                            if (chosenOption != 2)
                            {
                                resetColours();
                                chosenOption = 2;
                                option2.material = chosenMat;
                                clicked = true;
                            }
                            else
                            {
                                resetColours();
                                chosenOption = 0;
                                clicked = true;
                            }
                            break;
                        case "House":
                            if (chosenOption != 3)
                            {
                                resetColours();
                                chosenOption = 3;
                                option3.material = chosenMat;
                                clicked = true;
                            }
                            else
                            {
                                resetColours();
                                chosenOption = 0;
                                clicked = true;
                            }
                            break;
                        case "PathNPC":
                            if (chosenOption != 4)
                            {
                                resetColours();
                                chosenOption = 4;
                                option4.material = chosenMat;
                                clicked = true;
                            }
                            else
                            {
                                resetColours();
                                chosenOption = 0;
                                clicked = true;
                            }
                            break;
                        case "WanderNPC":
                            if (chosenOption != 5)
                            {
                                resetColours();
                                chosenOption = 5;
                                option5.material = chosenMat;
                                clicked = true;
                            }
                            else
                            {
                                resetColours();
                                chosenOption = 0;
                                clicked = true;
                            }
                            break;
                        case "Path1Select":
                            if (chosenOption != 6)
                            {
                                resetColours();
                                chosenOption = 6;
                                option6.material = chosenMat;
                                clicked = true;
                            }
                            else
                            {
                                resetColours();
                                chosenOption = 0;
                                clicked = true;
                            }
                            break;
                        case "Path2Select":
                            if (chosenOption != 7)
                            {
                                resetColours();
                                chosenOption = 7;
                                option7.material = chosenMat;
                                clicked = true;
                            }
                            else
                            {
                                resetColours();
                                chosenOption = 0;
                                clicked = true;
                            }
                            break;
                        case "Path3Select":
                            if (chosenOption != 8)
                            {
                                resetColours();
                                chosenOption = 8;
                                option8.material = chosenMat;
                                clicked = true;
                            }
                            else
                            {
                                resetColours();
                                chosenOption = 0;
                                clicked = true;
                            }
                            break;
                        case "CreateCustom":
                            if (chosenOption != 9)
                            {
                                resetColours();
                                chosenOption = 9;
                                option9.material = chosenMat;
                                clicked = true;
                                makingPath = true;
                                customPath.Clear();
                            }
                            else
                            {
                                resetColours();
                                chosenOption = 0;
                                clicked = true;
                                makingPath = false;
                            }
                            break;
                        case "Save":
                            if (chosenOption != 10)
                            {
                                chosenOption = 10;
                                clicked = true;
                            }
                            else
                            {
                                resetColours();
                                chosenOption = 0;
                                clicked = true;
                            }
                            break;
                        case "Map":
                            spawnObject(pos);
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
                        for (int i = 0; i < item.transform.childCount; i++)
                        {
                            if (item.transform.GetChild(i).name == "Rotation")
                            {
                                item.transform.GetChild(i).gameObject.SetActive(true);
                            }
                        }

                        item.transform.eulerAngles = new Vector3(item.transform.eulerAngles.x, item.transform.eulerAngles.y + 1, item.transform.eulerAngles.z);
                    }
                }
                else if (rotate.action.ReadValue<Vector2>().x < 0)
                {
                    rotating = true;
                    foreach (GameObject item in followObjects)
                    {
                        for(int i =0; i < item.transform.childCount;i++)
                        {
                            if(item.transform.GetChild(i).name == "Rotation")
                            {
                                item.transform.GetChild(i).gameObject.SetActive(true);
                            }
                        }
                      
                        item.transform.eulerAngles = new Vector3(item.transform.eulerAngles.x, item.transform.eulerAngles.y - 1, item.transform.eulerAngles.z);
                    }

                }
            }
           
        }
        else if (rightSelect.action.ReadValue<float>() <= 0.0f)
        {
            foreach (GameObject item in followObjects)
            {
                for (int i = 0; i < item.transform.childCount; i++)
                {
                    if (item.transform.GetChild(i).name == "Rotation")
                    {
                        item.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }

            }
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
        option4.material = notChosenMat;
        option5.material = notChosenMat;
        option6.material = notChosenMat;
        option7.material = notChosenMat;
        option8.material = notChosenMat;
        option9.material = notChosenMat;
    }

    private void spawnObject(Vector3 pos)
    {

        clicked = true;

        switch (chosenOption)
        {
            case 0:
                Collider[] hitColliders = Physics.OverlapSphere(pos, 1.0f);
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.tag != "Ground" && hitCollider.tag != "Plane" && hitCollider.tag != "Boundary")
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
            case 4:
                GameObject newPath = Instantiate(pathNPC, pos, Quaternion.identity);
                GetComponent<NPCCreator>().setNPC(newPath);
                break;
            case 5:
                Instantiate(wanderNPC, pos, Quaternion.identity);
                break;
            case 6:
                GetComponent<NPCCreator>().setPath(1);
                break;
            case 7:
                GetComponent<NPCCreator>().setPath(2);
                break;
            case 8:
                GetComponent<NPCCreator>().setPath(3);
                break;
            case 9:
                customPath.Add(pos);
                break;
            case 10:
                GetComponent<CSVWriter>().addFile("/path1.csv");
                GetComponent<CSVWriter>().WriteCSV(customPath);
                break;
        }

    }
}
