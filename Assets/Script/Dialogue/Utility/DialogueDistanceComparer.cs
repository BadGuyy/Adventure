using System.Collections.Generic;
using UnityEngine;

public class DialogueDistanceComparer : IComparer<(GameObject, float)>
{
    public int Compare((GameObject, float) a, (GameObject, float) b)
    {
        int result = a.Item2.CompareTo(b.Item2);
        return result;
    }
}