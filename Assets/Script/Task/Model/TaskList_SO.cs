using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TaskList", menuName = "Scriptable Objects/TaskList")]
public class TaskList_SO : ScriptableObject
{
    public List<TaskData> TaskList = new();
}
