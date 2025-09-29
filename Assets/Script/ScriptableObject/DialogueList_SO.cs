using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueList", menuName = "Scriptable Objects/DialogueList")]
public class DialogueList_SO : ScriptableObject
{
    public List<DialogueData> TaskList = new();
}
