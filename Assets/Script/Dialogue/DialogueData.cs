using System;

[Serializable]
public class DialogueData
{
    public int DialogueID;
    public string NPCName;
    public bool IsOption;
    public int OptionEndIndex;
    public string Content;
    public int Next;
}