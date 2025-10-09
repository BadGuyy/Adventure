using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private DialoguePanelController _dialoguePanelController;
    private DialoguePanelView _dialoguePanelView;
    private DialogueSelectionPanel _dialogueSelectionPanel;

    public static event Action<bool> OnDialogueStart;
    public static event Action<bool> OnDialogueEnd;

    private void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(string npcName)
    {
        // 开始对话，需要隐藏NPC对话交互按钮选择面板，停止玩家的移动和摄像机的旋转
        OnDialogueStart?.Invoke(false);
        if (_dialoguePanelView == null)
        {
            _dialoguePanelView = DialogueUIManager.Instance.OpenPanel(DialogueUIPanelName.DialoguePanel) as DialoguePanelView;
            _dialoguePanelController = _dialoguePanelView.GetComponent<DialoguePanelController>();
        }
        _dialoguePanelController.StartDialogue(npcName);
    }

    public void Next(DialogueData dialogueData = null)
    {
        _dialoguePanelController.Next(dialogueData);
    }

    public void ShowDialogueSelectionPanel(DialogueList_SO dialogueData, DialogueData currentDialogue)
    {
        if (_dialogueSelectionPanel == null)
        {
            _dialogueSelectionPanel = DialogueUIManager.Instance.OpenPanel(DialogueUIPanelName.DialogueSelectionPanel) as DialogueSelectionPanel;
        }
        _dialoguePanelView.SetDialogueAreaClickable(false);
        _dialogueSelectionPanel.ShowDialogueSelection(dialogueData, currentDialogue);
    }

    public void CloseDialogueSelectionPanel(DialogueData currentDialogue)
    {
        _dialoguePanelController.StartDialogue(currentDialogue);
        DialogueUIManager.Instance.ClosePanel(DialogueUIPanelName.DialogueSelectionPanel);
        _dialoguePanelView.SetDialogueAreaClickable(true);
    }

    public void CloseDialoguePanel()
    {
        DialogueUIManager.Instance.ClosePanel(DialogueUIPanelName.DialoguePanel);
        // 关闭对话，显示NPC对话交互按钮选择面板，恢复玩家的移动和摄像机的旋转
        OnDialogueEnd?.Invoke(true);
    }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        Instance = null;
    }
#endif
}