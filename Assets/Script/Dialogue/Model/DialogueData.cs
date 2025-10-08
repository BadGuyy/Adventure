using System;

[Serializable]
public class DialogueData
{
    internal int Phase;
    internal int DialogueID;
    internal string NPCName;
    internal bool IsOption;
    internal int OptionEndIndex;
    internal string Content;
    internal int Next;
    internal int NextPhase;
}