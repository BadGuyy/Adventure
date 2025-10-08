using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanelController : MonoBehaviour
{
    public Text _dialogueNPCName;
    private DialogueList_SO _dialogueData;
    private DialogueData _currentDialogue;
    private DialoguePanelView _dialoguePanelView;

    void Awake()
    {
        _dialoguePanelView = GetComponent<DialoguePanelView>();
    }

    // 开始对话
    public void StartDialogue(string npcName)
    {
        // 检查对话面板是否已经打开
        if (!DialogueUIManager.Instance.OpenedPanelDict.ContainsKey(DialogueUIPanelName.DialoguePanel))
        {
            DialogueUIManager.Instance.OpenPanel(DialogueUIPanelName.DialoguePanel);
        }

        _dialogueNPCName.text = npcName;
        _dialogueData = Resources.Load<DialogueList_SO>($"Dialogue/{_dialogueNPCName.text}");

        int currentPhase = SaveManager.Instance.LoadNPCDialoguePhase(npcName);
        int dialogueIndex = 0;
        for (int i = 0; i < _dialogueData.DialogueList.Count; i++)
        {
            if (_dialogueData.DialogueList[i].Phase == currentPhase)
            {
                dialogueIndex = i;
                break;
            }
        }
        _currentDialogue = _dialogueData.DialogueList[dialogueIndex];
        _dialoguePanelView.ShowDialogue(_currentDialogue);
    }

    // 在选择对话按钮的选项后回复对话面板的状态
    public void StartDialogue(DialogueData dialogueData)
    {
        _currentDialogue = dialogueData;
        _dialoguePanelView.ShowDialogue(_currentDialogue);
    }

    public void Next(DialogueData dialogueData)
    {
        if (dialogueData != null)
        {
            _currentDialogue = dialogueData;
        }
        // 检查文字是否已经显示完毕，没有的话直接全部显示
        if (_dialoguePanelView._dialogueTextTween != null && _dialoguePanelView._dialogueTextTween.IsActive())
        {
            _dialoguePanelView._dialogueTextTween.Complete(true);
        }
        else
        {
            // -1表示对话结束，关闭对话面板
            if (_currentDialogue.Next == -1)
            {
                // 在存档和当前Session里设置玩家对话的进度
                if (_currentDialogue.NextPhase != -1)
                {
                    SaveManager.Instance.SaveNPCDialogueData(_currentDialogue.NPCName, _currentDialogue.NextPhase);
                }
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
                _dialoguePanelView.ShowDialogue(_currentDialogue);
                _dialoguePanelView.HideDialogueCompleteHint();
            }
        }
    }
}