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


    private MeshRenderer _option1, _option2, _option3, _option4, _option5, _option6, _option7, _option8, _option9;

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
    private GameObject pathSphere;

    private Material notChosenMat;
    private Material chosenMat;

    private bool makingPath = false;
    private List<Vector3> customPath = new List<Vector3>();

    private Vector3[] positions = new Vector3[3];
    private List<GameObject> tempMarkers = new List<GameObject>();

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
        pathSphere = Resources.Load("Path_Sphere") as GameObject;

        notChosenMat = Resources.Load("notChosen_mat") as Material;
        chosenMat = Resources.Load("chosen_mat") as Material;

        //_mapEditor = GameObject.Find("MapEditor");

        setUpMeshRenderers();
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
                                SetChoice(1, _option1);
                            }
                            else
                            {
                                ResetChoice();
                            }
                            break;
                        case "Tree2":
                            if (chosenOption != 2)
                            {
                                SetChoice(2, _option2);
                            }
                            else
                            {
                                ResetChoice();
                            }
                            break;
                        case "House":
                            if (chosenOption != 3)
                            {
                                SetChoice(3, _option3);
                            }
                            else
                            {
                                ResetChoice();
                            }
                            break;
                        case "PathNPC":
                            if (chosenOption != 4)
                            {
                                SetChoice(4, _option4);
                            }
                            else
                            {
                                ResetChoice();
                            }
                            break;
                        case "WanderNPC":
                            if (chosenOption != 5)
                            {
                                SetChoice(5, _option5);
                            }
                            else
                            {
                                ResetChoice();
                            }
                            break;
                        case "Path1Select":
                            if (chosenOption != 6)
                            {
                                GetComponent<NPCCreator>().setPath(1);
                                SetChoice(6, _option6);
                            }
                            else
                            {
                                ResetChoice();
                            }
                            break;
                        case "Path2Select":
                            if (chosenOption != 7)
                            {
                                GetComponent<NPCCreator>().setPath(2);
                                SetChoice(7, _option7);
                            }
                            else
                            {
                                ResetChoice();
                            }
                            break;
                        case "Path3Select":
                            if (chosenOption != 8)
                            {
                                GetComponent<NPCCreator>().setPath(3);
                                SetChoice(8, _option8);
                            }
                            else
                            {
                                ResetChoice();
                            }
                            break;
                        case "CreateCustom":
                            if (chosenOption != 9)
                            {
                                SetChoice(9, _option9);
                                makingPath = true;
                                customPath.Clear();
                            }
                            else
                            {
                                ResetChoice();
                                makingPath = false;
                            }
                            break;
                        case "Save":
                            if (chosenOption != 10)
                            {
                                GetComponent<CSVWriter>().addFile();
                                CreateNewPath(GetComponent<CSVWriter>().WriteCSV(customPath));
                                makingPath = false;
                                customPath.Clear();
                                foreach(GameObject marker in tempMarkers)
                                {
                                    Destroy(marker);
                                }
                                tempMarkers.Clear();
                                clicked = true;
                            }
                            else
                            {
                                ResetChoice();
                            }
                            break;
                        case "BinPath1":
                            GetComponent<CSVWriter>().DeleteFile(5);
                            Destroy(GameObject.Find("Path5"));
                            break;
                        case "BinPath2":
                            GetComponent<CSVWriter>().DeleteFile(6);
                            Destroy(GameObject.Find("Path6"));
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
        _option1.material = notChosenMat;
        _option2.material = notChosenMat;
        _option3.material = notChosenMat;
        _option4.material = notChosenMat;
        _option5.material = notChosenMat;
        _option6.material = notChosenMat;
        _option7.material = notChosenMat;
        _option8.material = notChosenMat;
        _option9.material = notChosenMat;
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
                        //Debug.Log(hit.name);
                        //Debug.Log(hit.transform.parent);
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
                GameObject newPath = Instantiate(pathNPC, pos, Quaternion.identity,GameObject.Find("NPCS").transform);
                GetComponent<NPCCreator>().setNPC(newPath);
                break;
            case 5:
                Instantiate(wanderNPC, pos, Quaternion.identity, GameObject.Find("NPCS").transform);
                break;
            case 9:
                Collider[] hitColliders2 = Physics.OverlapSphere(pos, 1.0f);
                foreach (var hitCollider in hitColliders2)
                {
                    if ((hitCollider.tag == "Ground" || hitCollider.tag == "Plane") && makingPath)
                    {
                        customPath.Add(pos);
                        tempMarkers.Add(Instantiate(pathSphere, pos, Quaternion.identity));
                        Debug.Log("X" + pos.x);
                        Debug.Log("Y" + pos.y);
                        Debug.Log("Z" + pos.z);
                    }
                }
                break;
        }

    }

    private void setUpMeshRenderers()
    {
        _option1 = GameObject.Find("Tree1").GetComponent<MeshRenderer>();
        _option2 = GameObject.Find("Tree2").GetComponent<MeshRenderer>();
        _option3 = GameObject.Find("House").GetComponent<MeshRenderer>();
        _option4 = GameObject.Find("PathNPC").GetComponent<MeshRenderer>();
        _option5 = GameObject.Find("WanderNPC").GetComponent<MeshRenderer>();
        _option6 = GameObject.Find("Path1Select").GetComponent<MeshRenderer>();
        _option7 = GameObject.Find("Path2Select").GetComponent<MeshRenderer>();
        _option8 = GameObject.Find("Path3Select").GetComponent<MeshRenderer>();
        _option9 = GameObject.Find("CreateCustom").GetComponent<MeshRenderer>();
    }

    private void CreateNewPath(int pathID)
    {
        if(pathID != -1)
        {
            GameObject newPath = new GameObject("Path" + pathID);
            newPath.transform.parent = GameObject.Find("Paths").transform;

            GameObject newCell;
            for(int i = 0; i < customPath.Count;i++)
            {
                if(i == 0)
                {
                    newCell = new GameObject("Cell");
                }
                else
                {
                    newCell = new GameObject("Cell" + i);
                }
                newCell.transform.parent = newPath.transform;
                newCell.transform.localPosition = customPath[i];
                newCell.AddComponent<PathCellController>();
                Instantiate(pathSphere, newCell.transform.position, Quaternion.identity,newCell.transform);
            }
        }
    }

    private void ResetChoice()
    {
        resetColours();
        chosenOption = 0;
        clicked = true;
    }

    private void SetChoice(int option, MeshRenderer rend)
    {
        resetColours();
        chosenOption = option;
        rend.material = chosenMat;
        clicked = true;
    }
}
