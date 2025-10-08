using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueClickHandler : MonoBehaviour, IPointerClickHandler
{
    public bool isClickable = true;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClickable)
        {
            DialogueManager.Instance.Next();
        }
        else
        {
            return;
        }
    }

    void Update()
    {
        if (isClickable && Input.GetKeyDown(KeyCode.F))
        {
            DialogueManager.Instance.Next();
        }
    }
}