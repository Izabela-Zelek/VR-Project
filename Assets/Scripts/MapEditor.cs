using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Handles all interactions with the Map/Level Editor - including placing objects, rearranging objects and spawning NPCs
/// </summary>
public class MapEditor : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    public Transform centre;
    public Transform left;
    public Transform camCentre;
    public Transform camLeft;

    public InputActionProperty rightSelect;
    public InputActionProperty rotate;


    private MeshRenderer _option1, _option2, _option3, _option4, _option5, _option6, _option7, _option8, _option9, _option11, _option12, _option13;

    private float mapDistance;
    private float camDistance;
    private float multiplier;

    private int chosenOption = 0;

    private bool clicked = false;
    private bool rotating = false;
    private bool editPath = false;

    private bool unlockCustom5 = false;
    private bool unlockCustom6 = false;

    private int hoverNPCCount = 0;

    private GameObject hoverNPCRes;
    private GameObject hoverNPC;

    private GameObject tree;
    private GameObject tree2;
    private GameObject house;
    private GameObject pathNPC;
    private GameObject wanderNPC;
    private GameObject pathSphere;

    public GameObject CustomPath1;
    public GameObject BinPath1;
    public GameObject CustomPath2;
    public GameObject BinPath2;

    private Material notChosenMat;
    private Material chosenMat;

    private bool makingPath = false;
    private List<Vector3> customPath = new List<Vector3>();

    private List<GameObject> tempMarkers = new List<GameObject>();
    private int _tempMarkerCount = 0;
    private List<GameObject> followObjects = new List<GameObject>();

    private GameObject chosenMarker;
    public TextMeshPro loiterText;

    /// <summary>
    /// Calculates distance from centre of map to left side of map
    /// Calculates distance from centre of world to left side of world
    /// Calculates the multiplier - used for placing items in game world
    /// Loads all object prefabs from Resources folder
    /// Finds all Mesh Renderers for Map/Level Editor buttons
    /// </summary>
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

        setUpMeshRenderers();
    }
    /// <summary>
    /// Calculates distance of the selected point of the map and applies multiplier to determine location in game world
    /// Upon clicking with controller button, Calls ChooseOption function to determine whether a button was selected or the map
    /// If map was selected, grabs list of items clicked on and changes position to follow player ray
    /// Rotates selected items if joystick moved
    /// Turns off rotation gizmo upon letting go of controller button
    /// Positions hovering NPC to follow the player ray by the map
    /// </summary>
    private void Update()
    {
        if(hoverNPCCount == 1)
        {
            RaycastHit res;
            if (rayInteractor.TryGetCurrent3DRaycastHit(out res))
            {
                if (res.collider.name == "Map")
                {
                    Vector3 hitPoint = res.point; // the coordinate that the ray hits

                    hoverNPC.transform.position = new Vector3(hitPoint.x - 0.01f, hitPoint.y / 20 * 19, hitPoint.z);
                }
            }
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

                if (!clicked)
                {
                    ChooseOption(res, hitPoint, pos);
                }

                foreach (GameObject item in followObjects)
                {
                    item.transform.position = new Vector3(pos.x, item.transform.position.y, pos.z);
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
    /// <summary>
    /// Sets all buttons to grey 'unselected' colour
    /// </summary>
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
        _option11.material = notChosenMat;
        if(unlockCustom5)
        {
            _option12.material = notChosenMat;
        }
        if(unlockCustom6)
        {
            _option13.material = notChosenMat;
        }
    }

    /// <summary>
    /// Depending on chosen option, either adds selected object to a list so that it follows player ray (if clicked on map/level editor)
    /// or spawns selected object on the hit position (if clicked on an object button beforehand)
    /// </summary>
    /// <param name="pos"></param>
    private void spawnObject(Vector3 pos)
    {

        clicked = true;

        switch (chosenOption)
        {
            case 0:
                Collider[] hitColliders = Physics.OverlapSphere(pos, 1.5f);
                int count = 0;
                foreach (var hitCollider in hitColliders)
                {
                    if (hitColliders[count].tag == "marker")
                    {
                        chosenMarker = hitColliders[count].gameObject;
                        loiterText.text = chosenMarker.transform.parent.GetComponent<PathCellController>().GetLoiterTime().ToString();
                    }
                    if (hitColliders[count].tag != "Ground" && hitColliders[count].tag != "Plane" && hitColliders[count].tag != "Boundary" && hitColliders[count].tag != "Parent" && hitColliders[count].tag != "EntryPoints" && hitColliders[count].tag != "Path")
                    {
                        GameObject hit = hitColliders[count].gameObject;
                        while (hit.transform.parent != null)
                        {
                            if (hit.transform.parent.tag == "Parent")
                            {
                                break;
                            }
                            hit = hit.transform.parent.gameObject;
                        }
                        followObjects.Add(hit);
                        break;
                    }
                    else
                    {
                        count++;
                    }
                   
                }

                break;
            case 1:
                Instantiate(tree, pos, Quaternion.identity, GameObject.Find("Trees").transform);
                break;
            case 2:
                Instantiate(tree2, pos, Quaternion.identity, GameObject.Find("Trees").transform);
                break;
            case 3:
                Instantiate(house, pos, Quaternion.identity, GameObject.Find("Houses").transform);
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
                    if ((hitCollider.tag == "Ground" || hitCollider.tag == "Plane") && makingPath && _tempMarkerCount < 14)
                    {
                        tempMarkers.Add(Instantiate(pathSphere, pos, Quaternion.identity));
                        _tempMarkerCount++;
                    }
                }
                break;
            case 11:
                Collider[] hitColliders3 = Physics.OverlapSphere(pos, 1.0f);
                foreach (var hitCollider in hitColliders3)
                {
                    if (hitCollider.tag == "marker" )
                    {
                        followObjects.Add(hitCollider.gameObject);
                    }
                }
                break;
        }

    }

    /// <summary>
    /// Grabs and stores mesh renderers of each button on map/level editor
    /// </summary>
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
        _option11 = GameObject.Find("EditPath").GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Upon selecting option to save custom path, instantiates a path cell instance for each 'click' on the map editor which determines the new path directions
    /// </summary>
    /// <param name="pathID"></param>
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
                GameObject temp = Instantiate(pathSphere, newCell.transform.position, Quaternion.identity,newCell.transform);
                temp.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Resets all choices once selecting button is let go
    /// </summary>
    private void ResetChoice()
    {
        resetColours();
        chosenOption = 0;
        clicked = true;
    }

    /// <summary>
    /// Sets choice based on selected button
    /// </summary>
    /// <param name="option"></param>
    /// <param name="rend"></param>
    private void SetChoice(int option, MeshRenderer rend)
    {
        GetComponent<NPCCreator>().HideAllPath();
        resetColours();
        chosenOption = option;
        rend.material = chosenMat;
        clicked = true;
        if(hoverNPCCount != 0)
        {
            Destroy(hoverNPC);
            hoverNPCCount = 0;
        }
    }

    /// <summary>
    /// Upon creating custom path, adds the location of the new cell to a list
    /// </summary>
    private void UpdatePathPos()
    {
        foreach(GameObject marker in tempMarkers)
        {
            customPath.Add(marker.transform.localPosition);
        }
    }

    /// <summary>
    /// If created a new path, displays a new button which allows the player to reuse the custom path
    /// </summary>
    /// <param name="customID"></param>
    public void Unlock(int customID)
    {
        if(customID == 5)
        {
            unlockCustom5 = true;
            CustomPath1.SetActive(true);
            BinPath1.SetActive(true);
            _option12 = GameObject.Find("CustomPath1").GetComponent<MeshRenderer>();
        }
        else if (customID == 6)
        {
            unlockCustom6 = true;
            CustomPath2.SetActive(true);
            BinPath2.SetActive(true);
            _option13 = GameObject.Find("CustomPath2").GetComponent<MeshRenderer>();
        }
    }
    /// <summary>
    /// If deleted a custom path, hides the button to stop player from trying to access a deleted oatg
    /// </summary>
    /// <param name="customID"></param>
    public void Lock(int customID)
    {
        if (customID == 5)
        {
            unlockCustom5 = false;
            GameObject.Find("CustomPath1").gameObject.SetActive(false);
            GameObject.Find("BinPath1").gameObject.SetActive(false);
        }
        else if (customID == 6)
        {
            unlockCustom6 = false;
            GameObject.Find("CustomPath2").gameObject.SetActive(false);
            GameObject.Find("BinPath2").gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Based on whether player selecting a button on the map/level editor or the map editor itself, runs appropriate functions for each option
    /// eg. Upon selecting option to create custom path, stores all 'clicks' on map and spawns markers to display the path on the map
    /// </summary>
    /// <param name="res"></param>
    /// <param name="hitPoint"></param>
    /// <param name="pos"></param>
    public void ChooseOption(RaycastHit res, Vector3 hitPoint, Vector3 pos)
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
                    if (hoverNPCCount == 0)
                    {
                        hoverNPCCount++;
                        hoverNPCRes = Resources.Load("HoverDude") as GameObject;
                        hoverNPC = Instantiate(hoverNPCRes, hitPoint, Quaternion.Euler(0, 270, 0));
                    }
                }
                else
                {
                    Destroy(hoverNPC);
                    hoverNPCCount = 0;
                    ResetChoice();
                }
                break;
            case "WanderNPC":
                if (chosenOption != 5)
                {
                    SetChoice(5, _option5);
                    if (hoverNPCCount == 0)
                    {
                        hoverNPCCount++;
                        hoverNPCRes = Resources.Load("HoverDude") as GameObject;
                        hoverNPC = Instantiate(hoverNPCRes, hitPoint, Quaternion.Euler(0, 270, 0));
                    }
                }
                else
                {
                    Destroy(hoverNPC);
                    hoverNPCCount = 0;
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
                    UpdatePathPos();
                    GetComponent<CSVWriter>().addFile();
                    CreateNewPath(GetComponent<CSVWriter>().WriteCSV(customPath));
                    makingPath = false;
                    customPath.Clear();
                    foreach (GameObject marker in tempMarkers)
                    {
                        Destroy(marker);
                    }
                    tempMarkers.Clear();
                    _tempMarkerCount = 0;
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
            case "EditPath":
                if (chosenOption != 11)
                {
                    SetChoice(11, _option11);
                    editPath = true;
                }
                else
                {
                    ResetChoice();
                    editPath = false;
                }
                break;
            case "CustomPath1":
                if (chosenOption != 12 && unlockCustom5)
                {
                    GetComponent<NPCCreator>().setPath(5);
                    SetChoice(12, _option12);
                }
                else
                {
                    GetComponent<NPCCreator>().hidePath(5);
                    ResetChoice();
                }
                break;
            case "CustomPath2":
                if (chosenOption != 13 && unlockCustom6)
                {
                    GetComponent<NPCCreator>().setPath(6);
                    SetChoice(13, _option13);
                }
                else
                {
                    GetComponent<NPCCreator>().hidePath(6);
                    ResetChoice();
                }
                break;
            case "LeaveMap":
                GameObject.Find("GameManager").GetComponent<GameManager>().EnterMap(false);
                break;
            case "LoiterLeft":
                chosenMarker.transform.parent.GetComponent<PathCellController>().IncreaseLoiterTime(chosenMarker.transform.parent.GetComponent<PathCellController>().GetLoiterTime() - 1);
                loiterText.text = chosenMarker.transform.parent.GetComponent<PathCellController>().GetLoiterTime().ToString();
                break;
            case "LoiterRight":
                chosenMarker.transform.parent.GetComponent<PathCellController>().IncreaseLoiterTime(chosenMarker.transform.parent.GetComponent<PathCellController>().GetLoiterTime() + 1);
                loiterText.text = chosenMarker.transform.parent.GetComponent<PathCellController>().GetLoiterTime().ToString();
                break;
            case "Map":
                spawnObject(pos);
                break;
        }
    }
}
