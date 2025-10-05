using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class DialogueSelectionPanel : BasePanel
{
    private GameObject _dialogueSelectionButtonPrefab;

    private void Awake()
    {
        _dialogueSelectionButtonPrefab = Addressables.LoadAssetAsync<GameObject>("Assets/Prefab/UI/Button/DialogueSelectionButton.prefab").WaitForCompletion();
    }
    public void ShowDialogueSelection(DialogueList_SO dialogueData, DialogueData currentDialogue)
    {
        // 建立对话选项的列表
        List<DialogueData> dialogueList = new();
        while (currentDialogue.DialogueID != currentDialogue.OptionEndIndex)
        {
            dialogueList.Add(currentDialogue);
            currentDialogue = dialogueData.DialogueList[currentDialogue.DialogueID + 1];
        }
        dialogueList.Add(currentDialogue);

        // 打开对话选项面板
        if (!UIManager.Instance.OpenedPanelDict.ContainsKey(UIPanelName.DialogueSelectionPanel))
        {
            UIManager.Instance.OpenPanel(UIPanelName.DialogueSelectionPanel);
        }
        // 为选项面板添加选项按钮和按钮的监听器
        for (int i = 0; i < dialogueList.Count; i++)
        {
            int index = i;
            GameObject button = Instantiate(_dialogueSelectionButtonPrefab, transform);
            button.transform.Find("Text").GetComponent<Text>().text = dialogueList[i].Content;
            button.GetComponent<Button>().onClick.AddListener
            (
                () => DialogueManager.Instance.CloseDialogueSelectionPanel(dialogueData.DialogueList[dialogueList[index].Next])
            );
        }
    }

    public override void ClosePanel()
    {
        // 先执行父类的ClosePanel()方法
        base.ClosePanel();
        // 找到所有选项按钮，并移除所有监听器，并销毁
        foreach (Transform child in transform)
        {
            child.GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(child.gameObject);
        }
    }
}