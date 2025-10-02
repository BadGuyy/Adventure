using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIPanelName
{
    public const string InteractinoSelectionPanel = "InteractinoSelectionPanel";
    public const string DialoguePanel = "DialoguePanel";
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
    private HashSet<string> _dynamicCanvasDict;

    private void Awake()
    {
        Instance = this;
        LoadedPanelDict = new();
        OpenedPanelDict = new();
        UIRoot = GameObject.Find("/UI").transform;
        PanelPathDict = new()
        {
            { UIPanelName.InteractinoSelectionPanel, "Assets/Prefab/UI/Panel/InteractinoSelectionPanel.prefab" },
            { UIPanelName.DialoguePanel, "Assets/Prefab/UI/Panel/DialoguePanel.prefab" }
        };
        _dynamicCanvasDict = new()
        {
            UIPanelName.InteractinoSelectionPanel,
            UIPanelName.DialoguePanel
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

        Transform _rootCanvas;
        // 在指定的Canvas下打开Panel
        if (_dynamicCanvasDict.Contains(panelName))
        {
            Transform canvasTransform = UIRoot.Find(panelName.Replace("Panel", "Canvas"));
            _rootCanvas = canvasTransform;
        }
        else
        {
            Transform canvasTransform = UIRoot.Find("Canvas");
            _rootCanvas = canvasTransform;
        }

        // 使用缓存的预制件
        if (!LoadedPanelDict.TryGetValue(panelName, out GameObject panelPrefab))
        {
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
        if (!OpenedPanelDict.ContainsKey(UIPanelName.InteractinoSelectionPanel) && isAddButton)
        {
            OpenPanel(UIPanelName.InteractinoSelectionPanel);
        }

        InteractinoSelectionPanel panel = OpenedPanelDict[UIPanelName.InteractinoSelectionPanel] as InteractinoSelectionPanel;
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