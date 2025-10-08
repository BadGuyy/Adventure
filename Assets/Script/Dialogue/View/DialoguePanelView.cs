using DG.Tweening;
using Dialogue;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanelView : BasePanel
{
    public Text _dialogueContent;
    public GameObject _dialogueCompleteHint;
    [SerializeField] private DialogueClickHandler _dialogueClickHandler;
    internal Tween _dialogueTextTween;
    [Tooltip("每秒钟显示的文本数量")]
    [SerializeField] private int _dialogueTextPerSecond = 10;

    internal void ShowDialogue(DialogueData dialogueData)
    {
        float _duration = (float)dialogueData.Content.Length / _dialogueTextPerSecond;
        _dialogueContent.text = "";
        _dialogueTextTween = _dialogueContent.DOText(dialogueData.Content, _duration)
            .OnComplete(ShowDialogueCompleteHint);
    }

    private void ShowDialogueCompleteHint()
    {
        _dialogueCompleteHint.SetActive(true);
    }

    internal void HideDialogueCompleteHint()
    {
        _dialogueCompleteHint.SetActive(false);
    }

    public void SetDialogueAreaClickable(bool isClickable)
    {
        _dialogueClickHandler.isClickable = isClickable;
    }
}