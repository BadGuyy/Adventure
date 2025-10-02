using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public virtual void OpenPanel()
    {
        gameObject.SetActive(true);

        if (!UIManager.Instance.OpenedPanelDict.ContainsKey(name))
        {
            UIManager.Instance.OpenedPanelDict.Add(name, this);
        }
    }

    public virtual void ClosePanel()
    {
        gameObject.SetActive(false);

        if (UIManager.Instance.OpenedPanelDict.ContainsKey(name))
        {
            UIManager.Instance.OpenedPanelDict.Remove(name);
        }
    }
}