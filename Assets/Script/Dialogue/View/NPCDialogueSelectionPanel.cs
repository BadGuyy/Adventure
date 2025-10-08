using System.Collections.Generic;
using Dialogue;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class NPCDialogueSelectionPanel : BasePanel
{
    NPCDialogueSelectionPanelEventSystemHandler _NPCDialogueSelectionPanelEventSystemHandler;
    private DialogueDistanceComparer _dialogueDistanceComparer = new();

    private List<(GameObject, float)> _selectionButtons = new();
    private GameObject _interactinoSelectionButtonPrefab;

    void Awake()
    {
        _NPCDialogueSelectionPanelEventSystemHandler = GetComponent<NPCDialogueSelectionPanelEventSystemHandler>();
        _interactinoSelectionButtonPrefab = Addressables.LoadAssetAsync<GameObject>("Assets/Prefab/UI/Button/NPCDialogueChooseButton.prefab").WaitForCompletion();
    }

    void Start()
    {
        DialogueManager.OnDialogueStart += SetActive;
        DialogueManager.OnDialogueEnd += SetActive;
    }

    public void AddButton(float distance, string NPCName)
    {
        GameObject button = Instantiate(_interactinoSelectionButtonPrefab, transform);
        // 添加按钮点击触发对话事件
        button.GetComponent<Button>().onClick.AddListener
        (
            () => DialogueManager.Instance.StartDialogue(button.name)
        );
        // 为按钮添加选中相关功能
        _NPCDialogueSelectionPanelEventSystemHandler.AddSelectable(button.GetComponent<Selectable>());

        _selectionButtons.Add((button, distance));
        _selectionButtons.Sort(_dialogueDistanceComparer);
        button.name = NPCName;
        button.GetComponentInChildren<Text>().text = NPCName;
        PanelLayoutRefresh();
    }

    public void RemoveButton(GameObject button)
    {
        for (int i = 0; i < _selectionButtons.Count; i++)
        {
            if (_selectionButtons[i].Item1 == button)
            {
                _NPCDialogueSelectionPanelEventSystemHandler.RemoveSelectable(button.GetComponent<Selectable>());
                _selectionButtons.RemoveAt(i);
                _selectionButtons.Sort(_dialogueDistanceComparer);
                button.transform.SetParent(null, false);
                // 在摧毁按钮之前移除按钮点击触发对话事件
                button.GetComponent<Button>().onClick.RemoveAllListeners();
                Destroy(button);
                break;
            }
        }
        if (_selectionButtons.Count == 0)
        {
            DialogueUIManager.Instance.ClosePanel(DialogueUIPanelName.NPCDialogueSelectionPanel);
            return;
        }
        PanelLayoutRefresh();
    }

    private void PanelLayoutRefresh()
    {
        for (int i = 0; i < _selectionButtons.Count; i++)
        {
            _selectionButtons[i].Item1.transform.SetSiblingIndex(i);
        }

        LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
    }

    private void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    private void OnDestroy()
    {
        DialogueManager.OnDialogueStart -= SetActive;
        DialogueManager.OnDialogueEnd -= SetActive;
    }
}