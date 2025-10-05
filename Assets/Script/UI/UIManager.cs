using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIPanelName
{
    public const string NPCDialogueSelectionPanel = "NPCDialogueSelectionPanel";
    public const string DialoguePanel = "DialoguePanel";
    public const string DialogueSelectionPanel = "DialogueSelectionPanel";
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    /// <summary>
    /// 记录所有面板的读取路径
    /// </summary>
    public Dictionary<string, string> PanelPathDict;
    /// <summary>
    /// 记录已被加载到内存的预制件，实际存储的是实例化后的对象
    /// </summary>
    public Dictionary<string, GameObject> LoadedPanelDict;
    /// <summary>
    /// 记录所有被激活显示的面板
    /// </summary>
    public Dictionary<string, BasePanel> OpenedPanelDict;
    private Transform UIRoot;
    private Dictionary<string, string> _panel2CanvasDict;

    private void Awake()
    {
        Instance = this;
        LoadedPanelDict = new();
        OpenedPanelDict = new();
        UIRoot = GameObject.Find("/UI").transform;
        PanelPathDict = new()
        {
            { UIPanelName.NPCDialogueSelectionPanel, "Assets/Prefab/UI/Panel/NPCDialogueSelectionPanel.prefab" },
            { UIPanelName.DialoguePanel, "Assets/Prefab/UI/Panel/DialoguePanel.prefab" },
            { UIPanelName.DialogueSelectionPanel, "Assets/Prefab/UI/Panel/DialogueSelectionPanel.prefab" }
        };
        _panel2CanvasDict = new()
        {
            { UIPanelName.NPCDialogueSelectionPanel, "NPCDialogueSelectionCanvas" },
            { UIPanelName.DialoguePanel, "DialogueCanvas" },
            { UIPanelName.DialogueSelectionPanel, "DialogueCanvas" }
        };
    }

    public BasePanel OpenPanel(string panelName)
    {
        BasePanel basePanel;
        // 检查是否已经打开
        if (OpenedPanelDict.ContainsKey(panelName))
        {
            Debug.LogError("Panel: " + panelName + " already opened");
            return null;
        }

        // 检查路径是否存在
        if (!PanelPathDict.TryGetValue(panelName, out string panelPrefabPath))
        {
            Debug.LogError("Panel: " + panelName + " not found");
            return null;
        }


        // 使用缓存的预制件
        if (!LoadedPanelDict.TryGetValue(panelName, out GameObject panelPrefab))
        {
            // 在指定的Canvas下打开Panel
            Transform _rootCanvas;
            _rootCanvas = UIRoot.Find(_panel2CanvasDict[panelName]);

            panelPrefab = Addressables.LoadAssetAsync<GameObject>(panelPrefabPath).WaitForCompletion();
            GameObject panelObj = Instantiate(panelPrefab, _rootCanvas);
            panelObj.name = panelPrefab.name;
            LoadedPanelDict.Add(panelName, panelObj);
        }

        basePanel = LoadedPanelDict[panelName].GetComponent<BasePanel>();
        basePanel.OpenPanel();
        return basePanel;

    }

    public bool ClosePanel(string panelName)
    {
        if (!OpenedPanelDict.TryGetValue(panelName, out BasePanel basePanel))
        {
            Debug.LogError("Panel " + panelName + " 未打开");
            return false;
        }

        basePanel.ClosePanel();
        return true;
    }

    public void UpdateInteractinoSelectionPanel(float distance, bool isAddButton, string NPCName)
    {
        // 检查面板是否是UnActive状态
        if (!OpenedPanelDict.ContainsKey(UIPanelName.NPCDialogueSelectionPanel) && isAddButton)
        {
            OpenPanel(UIPanelName.NPCDialogueSelectionPanel);
        }

        NPCDialogueSelectionPanel panel = OpenedPanelDict[UIPanelName.NPCDialogueSelectionPanel] as NPCDialogueSelectionPanel;
        if (isAddButton)
        {
            panel.AddButton(distance, NPCName);
        }
        else
        {
            GameObject button = panel.transform.Find(NPCName)?.gameObject;
            panel.RemoveButton(button);
        }
    }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        Instance = null;
    }

#endif
}