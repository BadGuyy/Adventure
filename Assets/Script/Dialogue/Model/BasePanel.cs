using UnityEngine;

namespace Dialogue
{
    public class BasePanel : MonoBehaviour
    {
        public virtual void OpenPanel()
        {
            gameObject.SetActive(true);

            if (!DialogueUIManager.Instance.OpenedPanelDict.ContainsKey(name))
            {
                DialogueUIManager.Instance.OpenedPanelDict.Add(name, this);
            }
        }

        public virtual void ClosePanel()
        {
            gameObject.SetActive(false);

            if (DialogueUIManager.Instance.OpenedPanelDict.ContainsKey(name))
            {
                DialogueUIManager.Instance.OpenedPanelDict.Remove(name);
            }
        }
    }
}