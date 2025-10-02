using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InteractinoSelectionPanel : BasePanel
{
    private DialogueDistanceComparer _dialogueDistanceComparer;

    private List<(GameObject, float)> _selectionButtons;
    private GameObject _interactinoSelectionButtonPrefab;

    void Awake()
    {
        _dialogueDistanceComparer = new();
        _selectionButtons = new();
        _interactinoSelectionButtonPrefab = Addressables.LoadAssetAsync<GameObject>("Assets/Prefab/UI/Button/ChooseButton.prefab").WaitForCompletion();
    }

    public void AddButton(float distance, string NPCName)
    {
        GameObject button = Instantiate(_interactinoSelectionButtonPrefab, transform);
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
                _selectionButtons.RemoveAt(i);
                _selectionButtons.Sort(_dialogueDistanceComparer);
                button.transform.SetParent(null, false);
                break;
            }
        }
        if (_selectionButtons.Count == 0)
        {
            UIManager.Instance.ClosePanel(UIPanelName.InteractinoSelectionPanel);
            return;
        }
        PanelLayoutRefresh();
        Destroy(button);
    }

    private void PanelLayoutRefresh()
    {
        for (int i = 0; i < _selectionButtons.Count; i++)
        {
            _selectionButtons[i].Item1.transform.SetSiblingIndex(i);
        }

        LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
    }
}

public class DialogueDistanceComparer : IComparer<(GameObject, float)>
{
    public int Compare((GameObject, float) a, (GameObject, float) b)
    {
        int result = a.Item2.CompareTo(b.Item2);
        return result;
    }
}