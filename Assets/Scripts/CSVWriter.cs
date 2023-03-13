using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// A class which saves data to CSV file
/// </summary>
public class CSVWriter : MonoBehaviour
{
    private string _filename1 = "";
    private string _filename2 = "";
    private string _filename = "";

    private int _fileUsed = -1;

    private void Start()
    {
        _filename1 = Application.dataPath + "/path5.csv";
        _filename2 = Application.dataPath + "/path6.csv";
    }
    public void addFile()
    { 
        if(!File.Exists(_filename1))
        {
            _filename = _filename1;
            _fileUsed = 5;
        }
        else if(File.Exists(_filename1) && !File.Exists(_filename2))
        {
            _filename = _filename2;
            _fileUsed = 6;
        }
        else
        {
            _fileUsed = -1;
            _filename = null;
        }
    }

    public int WriteCSV(List<Vector3> path)
    {
        if (_filename != null)
        {
            TextWriter tw = new StreamWriter(_filename, true);

            for (int i = 0; i < path.Count; i++)
            {
                tw.WriteLine(path[i].x + "," + path[i].y + "," + path[i].z);
            }

            tw.Close();
        }

        return _fileUsed;
    }

    public void DeleteFile(int pathID)
    {
        switch(pathID)
        {
            case 5:
                File.Delete(_filename1);
                RefreshEditorProjectWindow();
                break;
            case 6:
                File.Delete(_filename2);
                RefreshEditorProjectWindow();
                break;
        }

    }

    void RefreshEditorProjectWindow()
    {
        #if UNITY_EDITOR
                 UnityEditor.AssetDatabase.Refresh();
        #endif
    }
}