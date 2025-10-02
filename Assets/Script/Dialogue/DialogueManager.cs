using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private GameObject _dialogueCanvas;
    private Text _dialogueNPCName;
    private Text _dialogueContent;
    private DialogueList_SO _dialogueData;
    private DialogueData _currentDialogue;

    private void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(GameObject npc)
    {
        if (_dialogueCanvas == null)
        {
            Init();
        }

        _dialogueCanvas.SetActive(true);
        _dialogueNPCName.text = npc.name;
        _dialogueData = Resources.Load<DialogueList_SO>($"Dialogue/{_dialogueNPCName.text}");
        Instance._currentDialogue = Instance._dialogueData.DialogueList[0];
        float _duration = _currentDialogue.Content.Length / 10;
        _dialogueContent.DOText(_currentDialogue.Content, _duration);
    }

    private void Init()
    {
        _dialogueCanvas = GameObject.Find("DialogueCanvas");
        DontDestroyOnLoad(_dialogueCanvas);
        Transform area = _dialogueCanvas.transform.Find("Area");
        _dialogueNPCName = area.Find("NPCName").GetComponent<Text>();
        _dialogueContent = area.Find("Content").Find("Dialogue").GetComponent<Text>();
    }

    public void Next()
    {
        _currentDialogue = _dialogueData.DialogueList[_currentDialogue.Next];
    }

#if UNITY_EDITOR
    private void Oestroy()
    {
        Instance = null;        
    }
#endif
}