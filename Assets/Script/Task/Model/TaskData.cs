using System;

[Serializable]
public class TaskData
{
    public uint TaskID;
    public uint TaskType;
    public string TaskName;
    public string TaskDescription;
    public string PublishedBy;
    public string TaskTargetLocation;
    public bool IsTaskCompleted;
}
