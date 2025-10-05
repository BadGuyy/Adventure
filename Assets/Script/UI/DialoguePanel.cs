using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : BasePanel
{
    public Text DialogueNPCName;
    public Text DialogueContent;
    public GameObject DialogueCompleteHint;
    [SerializeField] private DialogueClickHandler _dialogueClickHandler;
    private DialogueList_SO _dialogueData;
    private DialogueData _currentDialogue;
    private Tween _dialogueTextTween;
    [SerializeField] private int _dialogueTextPerSecond = 10;

    void Awake()
    {
        // 把对话的文字动画的生命周期绑定到对话面板的生命周期
        // _dialogueTextTween.SetLink(_dialogueContent.gameObject);
    }

    // 开始对话
    public void StartDialogue(GameObject npc)
    {
        // 检查对话面板是否已经打开
        if (!UIManager.Instance.OpenedPanelDict.ContainsKey(UIPanelName.DialoguePanel))
        {
            UIManager.Instance.OpenPanel(UIPanelName.DialoguePanel);
        }

        DialogueNPCName.text = npc.name;
        _dialogueData = Resources.Load<DialogueList_SO>($"Dialogue/{DialogueNPCName.text}");

        _currentDialogue = _dialogueData.DialogueList[0];
        ShowDialogue(_currentDialogue);
    }

    // 在选择对话按钮的选项后回复对话面板的状态
    public void StartDialogue(DialogueData dialogueData)
    {
        _currentDialogue = dialogueData;
        ShowDialogue(_currentDialogue);
    }

    private void ShowDialogue(DialogueData dialogueData)
    {
        float _duration = (float)dialogueData.Content.Length / _dialogueTextPerSecond;
        DialogueContent.text = "";
        _dialogueTextTween = DialogueContent.DOText(dialogueData.Content, _duration)
            .OnComplete(ShowDialogueCompleteHint);
    }

    public void Next(DialogueData dialogueData)
    {
        if (dialogueData != null)
        {
            _currentDialogue = dialogueData;
        }
        // 检查文字是否已经显示完毕
        if (_dialogueTextTween != null && _dialogueTextTween.IsActive())
        {
            _dialogueTextTween.Complete(true);
        }
        else
        {
            // -1表示对话结束，关闭对话面板
            if (_currentDialogue.Next == -1)
            {
                DialogueManager.Instance.CloseDialoguePanel();
                return;
            }
            // 如果是下条对话是选项，则显示选项面板，否则显示下一条对话
            _currentDialogue = _dialogueData.DialogueList[_currentDialogue.Next];
            if (_currentDialogue.IsOption)
            {
                DialogueManager.Instance.ShowDialogueSelectionPanel(_dialogueData, _currentDialogue);
            }
            else
            {
                ShowDialogue(_currentDialogue);
                HideDialogueCompleteHint();
            }
        }
    }

    private void ShowDialogueCompleteHint()
    {
        DialogueCompleteHint.SetActive(true);
    }

    private void HideDialogueCompleteHint()
    {
        DialogueCompleteHint.SetActive(false);
    }

    public void SetDialogueAreaClickable(bool isClickable)
    {
        _dialogueClickHandler.isClickable = isClickable;
    }
}