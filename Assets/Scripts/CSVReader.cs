using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVReader : MonoBehaviour
{
    private TextAsset _textAssetData;
    private List<Vector3> _customPath = new List<Vector3>();
    private string _filename1 = "";
    private string _filename2 = "";
    private GameObject _pathSphere;

    private void Start()
    {
        _pathSphere = Resources.Load("Path_Sphere") as GameObject;

        _filename1 = Application.dataPath + "/Resources/path5.csv";
        _filename2 = Application.dataPath + "/Resources/path6.csv";

        if (File.Exists(_filename1))
        {
            _customPath.Clear();
            _textAssetData = Resources.Load<TextAsset>("path5");
            ReadCSV();
            CreateNewPath(5);
        }
        if (File.Exists(_filename2))
        {
            _customPath.Clear();
            _textAssetData = Resources.Load<TextAsset>("path6");
            ReadCSV();
            CreateNewPath(6);
        }
    }

    public void ReadCSV()
    {
        string[] data = _textAssetData.text.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
        string[] line;

        for (int i = 0; i < data.Length - 1; i++)
        {
            line = data[i].ToString().Split(",");
            Vector3 newPos = new Vector3();
            newPos.x = float.Parse(line[0]);
            newPos.y = float.Parse(line[1]);
            newPos.z = float.Parse(line[2]);
            _customPath.Add(newPos);
        }
    }

    private void CreateNewPath(int pathID)
    {
        if (pathID != -1)
        {
            GameObject newPath = new GameObject("Path" + pathID);
            newPath.transform.parent = GameObject.Find("Paths").transform;

            GameObject newCell;
            for (int i = 0; i < _customPath.Count; i++)
            {
                if (i == 0)
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
                GameObject temp = Instantiate(_pathSphere, newCell.transform.position, Quaternion.identity, newCell.transform);
                temp.SetActive(false);
            }
        }
    }
}