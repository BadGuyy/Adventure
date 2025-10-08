using UnityEngine;

public class PlayerDialogue : MonoBehaviour
{
    private LayerMask _NPCLayer;

    void Awake()
    {
        _NPCLayer = LayerMask.NameToLayer("NPC");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _NPCLayer)
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);
            DialogueUIManager.Instance.UpdateInteractinoSelectionPanel(distance, true, other.gameObject.name);
            DialogueUIManager.Instance.SwitchCursorVisibility(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == _NPCLayer)
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);
            DialogueUIManager.Instance.UpdateInteractinoSelectionPanel(distance, false, other.gameObject.name);
            DialogueUIManager.Instance.SwitchCursorVisibility(false);
        }
    }
}