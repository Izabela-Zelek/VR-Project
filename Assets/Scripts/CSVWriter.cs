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
    private string _filename = "";


    public void addFile(string loc)
    {
        _filename = Application.dataPath + loc;
    }

    public void WriteCSV(List<Vector3> path)
    {
        TextWriter tw = new StreamWriter(_filename, true);

        for (int i = 0; i < path.Count; i++)
        {
            tw.WriteLine(path[i].x + "," + path[i].y + "," + path[i].z);
        }

        tw.Close();
    }
}