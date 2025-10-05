using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    DialoguePanel _dialoguePanel;
    private DialogueSelectionPanel _dialogueSelectionPanel;

    public static event Action<bool> OnDialogueStart;
    public static event Action<bool> OnDialogueEnd;

    private void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(GameObject npcDialogueInteractionButton)
    {
        // 开始对话，需要隐藏NPC对话交互按钮选择面板，停止玩家的移动和摄像机的旋转
        OnDialogueStart?.Invoke(false);
        if (_dialoguePanel == null)
        {
            _dialoguePanel = UIManager.Instance.OpenPanel(UIPanelName.DialoguePanel) as DialoguePanel;
        }
        _dialoguePanel.StartDialogue(npcDialogueInteractionButton);
    }

    public void Next(DialogueData dialogueData = null)
    {
        _dialoguePanel.Next(dialogueData);
    }

    public void ShowDialogueSelectionPanel(DialogueList_SO dialogueData, DialogueData currentDialogue)
    {
        if (_dialogueSelectionPanel == null)
        {
            _dialogueSelectionPanel = UIManager.Instance.OpenPanel(UIPanelName.DialogueSelectionPanel) as DialogueSelectionPanel;
        }
        _dialoguePanel.SetDialogueAreaClickable(false);
        _dialogueSelectionPanel.ShowDialogueSelection(dialogueData, currentDialogue);
    }

    public void CloseDialogueSelectionPanel(DialogueData currentDialogue)
    {
        _dialoguePanel.StartDialogue(currentDialogue);
        UIManager.Instance.ClosePanel(UIPanelName.DialogueSelectionPanel);
        _dialoguePanel.SetDialogueAreaClickable(true);
    }

    public void CloseDialoguePanel()
    {
        UIManager.Instance.ClosePanel(UIPanelName.DialoguePanel);
        // 关闭对话，显示NPC对话交互按钮选择面板，恢复玩家的移动和摄像机的旋转
        OnDialogueEnd?.Invoke(true);
    }

#if UNITY_EDITOR
    private void Oestroy()
    {
        Instance = null;
    }
#endif
}