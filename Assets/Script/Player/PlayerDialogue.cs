using System;
using UnityEngine;

public class PlayerDialogue : MonoBehaviour
{
    private GameObject _currentNPC;

    private void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            DialogueManager.Instance.StartDialogue(_currentNPC);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);
            UIManager.Instance.UpdateInteractinoSelectionPanel(distance, true, other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);
            UIManager.Instance.UpdateInteractinoSelectionPanel(distance, false, other.gameObject.name);
        }
    }
}