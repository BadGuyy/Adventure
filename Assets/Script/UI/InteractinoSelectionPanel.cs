using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractinoSelectionPanel : BasePanel
{
    MenuEventSystemHandler _menuEventSystemHandler;
    private DialogueDistanceComparer _dialogueDistanceComparer = new();

    private List<(GameObject, float)> _selectionButtons = new();
    private GameObject _interactinoSelectionButtonPrefab;

    void Awake()
    {
        _menuEventSystemHandler = GetComponent<MenuEventSystemHandler>();
        _interactinoSelectionButtonPrefab = Addressables.LoadAssetAsync<GameObject>("Assets/Prefab/UI/Button/ChooseButton.prefab").WaitForCompletion();
    }

    public void AddButton(float distance, string NPCName)
    {
        GameObject button = Instantiate(_interactinoSelectionButtonPrefab, transform);
        _menuEventSystemHandler.AddSelectable(button.GetComponent<Selectable>());
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
                _menuEventSystemHandler.RemoveSelectable(button.GetComponent<Selectable>());
                _selectionButtons.RemoveAt(i);
                _selectionButtons.Sort(_dialogueDistanceComparer);
                button.transform.SetParent(null, false);
                Destroy(button);
                break;
            }
        }
        if (_selectionButtons.Count == 0)
        {
            UIManager.Instance.ClosePanel(UIPanelName.InteractinoSelectionPanel);
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
}

public class DialogueDistanceComparer : IComparer<(GameObject, float)>
{
    public int Compare((GameObject, float) a, (GameObject, float) b)
    {
        int result = a.Item2.CompareTo(b.Item2);
        return result;
    }
}