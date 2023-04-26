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
    public XRRayInteractor RayInteractor;
    public Transform Centre;
    public Transform Left;
    public Transform CamCentre;
    public Transform CamLeft;
    public InputActionProperty RightSelect;
    public InputActionProperty Rotate;
    public GameObject CustomPath1;
    public GameObject BinPath1;
    public GameObject CustomPath2;
    public GameObject BinPath2;
    public TextMeshPro LoiterText;
    public bool EditPath { get => _editPath; set => _editPath = value; }

    private MeshRenderer _option1, _option2, _option3, _option4, _option5, _option6, _option7, _option8, _option9, _option11, _option12, _option13;
    private float _mapDistance;
    private float _camDistance;
    private float _multiplier;
    private int _chosenOption = 0;
    private bool _clicked = false;
    private bool _rotating = false;
    private bool _editPath = false;
    private bool _unlockCustom5 = false;
    private bool _unlockCustom6 = false;
    private int _hoverNPCCount = 0;
    private GameObject _hoverNPCRes;
    private GameObject _hoverNPC;
    private GameObject _tree;
    private GameObject _tree2;
    private GameObject _house;
    private GameObject _pathNPC;
    private GameObject _wanderNPC;
    private GameObject _pathSphere;
    private Material _notChosenMat;
    private Material _chosenMat;
    private bool _makingPath = false;
    private List<Vector3> _customPath = new List<Vector3>();
    private List<GameObject> _tempMarkers = new List<GameObject>();
    private int _tempMarkerCount = 0;
    private List<GameObject> _followObjects = new List<GameObject>();
    private GameObject _chosenMarker;

    /// <summary>
    /// Calculates distance from centre of map to left side of map
    /// Calculates distance from centre of world to left side of world
    /// Calculates the multiplier - used for placing items in game world
    /// Loads all object prefabs from Resources folder
    /// Finds all Mesh Renderers for Map/Level Editor buttons
    /// </summary>
    private void Start()
    {
        _mapDistance = Vector3.Distance(Centre.position, Left.position);
        _camDistance = Vector3.Distance(CamCentre.position, CamLeft.position);

        _multiplier = _camDistance / _mapDistance;

        _tree = Resources.Load("Tree_1_1") as GameObject;
        _tree2 = Resources.Load("Tree_1_2") as GameObject;
        _house = Resources.Load("ShopGuyHouse") as GameObject;
        _pathNPC = Resources.Load("PathNPC") as GameObject;
        _wanderNPC = Resources.Load("WanderNPC") as GameObject;
        _pathSphere = Resources.Load("Path_Sphere") as GameObject;

        _notChosenMat = Resources.Load("notChosen_mat") as Material;
        _chosenMat = Resources.Load("chosen_mat") as Material;

        SetUpMeshRenderers();
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
        if(_hoverNPCCount == 1)
        {
            RaycastHit res;
            if (RayInteractor.TryGetCurrent3DRaycastHit(out res))
            {
                if (res.collider.name == "Map")
                {
                    Vector3 hitPoint = res.point; // the coordinate that the ray hits

                    _hoverNPC.transform.position = new Vector3(hitPoint.x - 0.01f, hitPoint.y / 20 * 19, hitPoint.z);
                }
            }
        }
        if (RightSelect.action.ReadValue<float>() >= 0.1f)
        {
            RaycastHit res;
            if (RayInteractor.TryGetCurrent3DRaycastHit(out res))
            {
                Vector3 hitPoint = res.point; // the coordinate that the ray hits

                float yDistance = Mathf.Sqrt((hitPoint.y - Centre.position.y) * (hitPoint.y - Centre.position.y));
                float zDistance = Mathf.Sqrt((hitPoint.z - Centre.position.z) * (hitPoint.z - Centre.position.z));

                if (hitPoint.z > Centre.position.z)
                {
                    zDistance = -zDistance;
                }
                zDistance = zDistance * _multiplier;

                if (hitPoint.y > Centre.position.y)
                {
                    yDistance = -yDistance;
                }
                yDistance = yDistance * _multiplier;

                Vector3 pos = new Vector3(CamCentre.position.x + yDistance, 0.0f, CamCentre.position.z + zDistance);

                if (!_clicked)
                {
                    ChooseOption(res, hitPoint, pos);
                }

                foreach (GameObject item in _followObjects)
                {
                    item.transform.position = new Vector3(pos.x, item.transform.position.y, pos.z);
                }

                if (Rotate.action.ReadValue<Vector2>().x > 0)
                {
                    _rotating = true;
                    foreach (GameObject item in _followObjects)
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
                else if (Rotate.action.ReadValue<Vector2>().x < 0)
                {
                    _rotating = true;
                    foreach (GameObject item in _followObjects)
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
        else if (RightSelect.action.ReadValue<float>() <= 0.0f)
        {
            foreach (GameObject item in _followObjects)
            {
                for (int i = 0; i < item.transform.childCount; i++)
                {
                    if (item.transform.GetChild(i).name == "Rotation")
                    {
                        item.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }

            }
            _clicked = false;
            _followObjects.Clear();
            _rotating = false;
        }


    }
    /// <summary>
    /// Sets all buttons to grey 'unselected' colour
    /// </summary>
    private void ResetColours()
    {
        _option1.material = _notChosenMat;
        _option2.material = _notChosenMat;
        _option3.material = _notChosenMat;
        _option4.material = _notChosenMat;
        _option5.material = _notChosenMat;
        _option6.material = _notChosenMat;
        _option7.material = _notChosenMat;
        _option8.material = _notChosenMat;
        _option9.material = _notChosenMat;
        _option11.material = _notChosenMat;
        if(_unlockCustom5)
        {
            _option12.material = _notChosenMat;
        }
        if(_unlockCustom6)
        {
            _option13.material = _notChosenMat;
        }
    }

    /// <summary>
    /// Depending on chosen option, either adds selected object to a list so that it follows player ray (if clicked on map/level editor)
    /// or spawns selected object on the hit position (if clicked on an object button beforehand)
    /// </summary>
    /// <param name="pos"></param>
    private void SpawnObject(Vector3 pos)
    {

        _clicked = true;

        switch (_chosenOption)
        {
            case 0:
                Collider[] hitColliders = Physics.OverlapSphere(pos, 1.5f);
                int count = 0;
                foreach (var hitCollider in hitColliders)
                {
                    if (hitColliders[count].tag == "marker")
                    {
                        _chosenMarker = hitColliders[count].gameObject;
                        LoiterText.text = _chosenMarker.transform.parent.GetComponent<PathCellController>().GetLoiterTime().ToString();
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
                        _followObjects.Add(hit);
                        break;
                    }
                    else
                    {
                        count++;
                    }
                   
                }

                break;
            case 1:
                Instantiate(_tree, pos, Quaternion.identity, GameObject.Find("Trees").transform);
                break;
            case 2:
                Instantiate(_tree2, pos, Quaternion.identity, GameObject.Find("Trees").transform);
                break;
            case 3:
                Instantiate(_house, pos, Quaternion.identity, GameObject.Find("Houses").transform);
                break;
            case 4:
                GameObject newPath = Instantiate(_pathNPC, pos, Quaternion.identity,GameObject.Find("NPCS").transform);
                GetComponent<NPCCreator>().setNPC(newPath);
                break;
            case 5:
                Instantiate(_wanderNPC, pos, Quaternion.identity, GameObject.Find("NPCS").transform);
                break;
            case 9:
                Collider[] hitColliders2 = Physics.OverlapSphere(pos, 1.0f);
                foreach (var hitCollider in hitColliders2)
                {
                    if ((hitCollider.tag == "Ground" || hitCollider.tag == "Plane") && _makingPath && _tempMarkerCount < 14)
                    {
                        _tempMarkers.Add(Instantiate(_pathSphere, pos, Quaternion.identity));
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
                        _followObjects.Add(hitCollider.gameObject);
                    }
                }
                break;
        }

    }

    /// <summary>
    /// Grabs and stores mesh renderers of each button on map/level editor
    /// </summary>
    private void SetUpMeshRenderers()
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
            for(int i = 0; i < _customPath.Count;i++)
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
                newCell.transform.localPosition = _customPath[i];
                newCell.AddComponent<PathCellController>();
                GameObject temp = Instantiate(_pathSphere, newCell.transform.position, Quaternion.identity,newCell.transform);
                temp.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Resets all choices once selecting button is let go
    /// </summary>
    private void ResetChoice()
    {
        ResetColours();
        _chosenOption = 0;
        _clicked = true;
    }

    /// <summary>
    /// Sets choice based on selected button
    /// </summary>
    /// <param name="option"></param>
    /// <param name="rend"></param>
    private void SetChoice(int option, MeshRenderer rend)
    {
        GetComponent<NPCCreator>().hideAllPath();
        ResetColours();
        _chosenOption = option;
        rend.material = _chosenMat;
        _clicked = true;
        if(_hoverNPCCount != 0)
        {
            Destroy(_hoverNPC);
            _hoverNPCCount = 0;
        }
    }

    /// <summary>
    /// Upon creating custom path, adds the location of the new cell to a list
    /// </summary>
    private void UpdatePathPos()
    {
        foreach(GameObject marker in _tempMarkers)
        {
            _customPath.Add(marker.transform.localPosition);
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
            _unlockCustom5 = true;
            CustomPath1.SetActive(true);
            BinPath1.SetActive(true);
            _option12 = GameObject.Find("CustomPath1").GetComponent<MeshRenderer>();
        }
        else if (customID == 6)
        {
            _unlockCustom6 = true;
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
            _unlockCustom5 = false;
            GameObject.Find("CustomPath1").gameObject.SetActive(false);
            GameObject.Find("BinPath1").gameObject.SetActive(false);
        }
        else if (customID == 6)
        {
            _unlockCustom6 = false;
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
                if (_chosenOption != 1)
                {
                    SetChoice(1, _option1);
                }
                else
                {
                    ResetChoice();
                }
                break;
            case "Tree2":
                if (_chosenOption != 2)
                {
                    SetChoice(2, _option2);
                }
                else
                {
                    ResetChoice();
                }
                break;
            case "House":
                if (_chosenOption != 3)
                {
                    SetChoice(3, _option3);
                }
                else
                {
                    ResetChoice();
                }
                break;
            case "PathNPC":
                if (_chosenOption != 4)
                {
                    SetChoice(4, _option4);
                    if (_hoverNPCCount == 0)
                    {
                        _hoverNPCCount++;
                        _hoverNPCRes = Resources.Load("HoverDude") as GameObject;
                        _hoverNPC = Instantiate(_hoverNPCRes, hitPoint, Quaternion.Euler(0, 270, 0));
                    }
                }
                else
                {
                    Destroy(_hoverNPC);
                    _hoverNPCCount = 0;
                    ResetChoice();
                }
                break;
            case "WanderNPC":
                if (_chosenOption != 5)
                {
                    SetChoice(5, _option5);
                    if (_hoverNPCCount == 0)
                    {
                        _hoverNPCCount++;
                        _hoverNPCRes = Resources.Load("HoverDude") as GameObject;
                        _hoverNPC = Instantiate(_hoverNPCRes, hitPoint, Quaternion.Euler(0, 270, 0));
                    }
                }
                else
                {
                    Destroy(_hoverNPC);
                    _hoverNPCCount = 0;
                    ResetChoice();
                }
                break;
            case "Path1Select":
                if (_chosenOption != 6)
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
                if (_chosenOption != 7)
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
                if (_chosenOption != 8)
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
                if (_chosenOption != 9)
                {
                    SetChoice(9, _option9);
                    _makingPath = true;
                    _customPath.Clear();
                }
                else
                {
                    ResetChoice();
                    _makingPath = false;
                }
                break;
            case "Save":
                if (_chosenOption != 10)
                {
                    UpdatePathPos();
                    GetComponent<CSVWriter>().addFile();
                    CreateNewPath(GetComponent<CSVWriter>().WriteCSV(_customPath));
                    _makingPath = false;
                    _customPath.Clear();
                    foreach (GameObject marker in _tempMarkers)
                    {
                        Destroy(marker);
                    }
                    _tempMarkers.Clear();
                    _tempMarkerCount = 0;
                    _clicked = true;
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
                if (_chosenOption != 11)
                {
                    SetChoice(11, _option11);
                    EditPath = true;
                }
                else
                {
                    ResetChoice();
                    EditPath = false;
                }
                break;
            case "CustomPath1":
                if (_chosenOption != 12 && _unlockCustom5)
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
                if (_chosenOption != 13 && _unlockCustom6)
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
                _chosenMarker.transform.parent.GetComponent<PathCellController>().IncreaseLoiterTime(_chosenMarker.transform.parent.GetComponent<PathCellController>().GetLoiterTime() - 1);
                LoiterText.text = _chosenMarker.transform.parent.GetComponent<PathCellController>().GetLoiterTime().ToString();
                break;
            case "LoiterRight":
                _chosenMarker.transform.parent.GetComponent<PathCellController>().IncreaseLoiterTime(_chosenMarker.transform.parent.GetComponent<PathCellController>().GetLoiterTime() + 1);
                LoiterText.text = _chosenMarker.transform.parent.GetComponent<PathCellController>().GetLoiterTime().ToString();
                break;
            case "Map":
                SpawnObject(pos);
                break;
        }
    }
}
