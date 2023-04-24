using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles keeping track of the amount of children currently on a plant stem
/// </summary>
public class MultiFruitStemController : MonoBehaviour
{
    public int childCount;

    /// <summary>
    /// Decrements a child upon harvest
    /// </summary>
    public void MinusChild()
    {
        childCount--;
    }
}
