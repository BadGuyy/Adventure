using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NPCDialogueSelectionPanelEventSystemHandler : MonoBehaviour
{
    [Header("References")]
    public List<Selectable> Selectables = new();
    protected Selectable _lastSelected;

    [Header("Controls")]
    [SerializeField] protected InputActionReference _navigateAction;

    [Header("Animations")]
    [SerializeField] protected float _selectedAnimationScale = 1.2f;
    [SerializeField] protected float _selectedAnimationDuration = 0.2f;
    [SerializeField] protected List<GameObject> _animationExcusions = new();

    [Header("Sounds")]
    [SerializeField] protected UnityEvent _soundEvent;

    protected Dictionary<Selectable, Vector3> _selectableOriginalScales = new();

    protected Tween _scaleUpTween;
    protected Tween _scaleDownTween;

    private void OnEnable()
    {
        _navigateAction.action.performed += OnNavigate;

        // 确保所有可选对象都恢复到原大小
        foreach (var selectable in Selectables)
        {
            selectable.transform.localScale = _selectableOriginalScales[selectable];
        }
    }

    private void OnDisable()
    {
        _navigateAction.action.performed -= OnNavigate;

        _scaleUpTween?.Kill();
        _scaleDownTween?.Kill();
    }


    public virtual void AddSelectable(Selectable selectable)
    {
        Selectables.Add(selectable);
        AddSelectedListeners(selectable);
        _selectableOriginalScales.Add(selectable, selectable.transform.localScale);
        if (Selectables.Count == 1)
        {
            EventSystem.current.SetSelectedGameObject(selectable.gameObject);
        }
    }

    public virtual void RemoveSelectable(Selectable selectable)
    {
        if (selectable.gameObject == EventSystem.current.currentSelectedGameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
            _lastSelected = null;
        }
        
        _scaleUpTween?.Kill();
        _scaleDownTween?.Kill();
        Selectables.Remove(selectable);
        _selectableOriginalScales.Remove(selectable);
    }

    public virtual void AddSelectedListeners(Selectable selectable)
    {
        // Add listeners for select events
        EventTrigger trigger = selectable.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = selectable.gameObject.AddComponent<EventTrigger>();
        }

        // Add select event
        EventTrigger.Entry selectEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Select
        };
        selectEntry.callback.AddListener(OnSelect);
        trigger.triggers.Add(selectEntry);

        // Add deselect event
        EventTrigger.Entry deselectEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Deselect
        };
        deselectEntry.callback.AddListener(OnDeselect);
        trigger.triggers.Add(deselectEntry);

        // Add pointer enter event
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnterEntry.callback.AddListener(OnPointerEnter);
        trigger.triggers.Add(pointerEnterEntry);

        // Add pointer exit event
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        pointerExitEntry.callback.AddListener(OnPointerExit);
        trigger.triggers.Add(pointerExitEntry);
    }

    protected virtual void OnSelect(BaseEventData eventData)
    {
        _soundEvent?.Invoke();

        if (_animationExcusions.Contains(eventData.selectedObject))
            return;

        _lastSelected = eventData.selectedObject.GetComponent<Selectable>();

        Vector3 newScale = _selectableOriginalScales[eventData.selectedObject.GetComponent<Selectable>()] * _selectedAnimationScale;
        _scaleUpTween = eventData.selectedObject.transform.DOScale(newScale, _selectedAnimationDuration)
           .SetLink(eventData.selectedObject);
    }

    protected virtual void OnDeselect(BaseEventData eventData)
    {
        if (_animationExcusions.Contains(eventData.selectedObject))
            return;

        Selectable selectable = eventData.selectedObject.GetComponent<Selectable>();
        _scaleDownTween = selectable.transform.DOScale(_selectableOriginalScales[selectable], _selectedAnimationDuration)
           .SetLink(eventData.selectedObject);
    }

    protected virtual void OnPointerEnter(BaseEventData eventData)
    {
        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData != null)
        {
            Selectable selectable = pointerEventData.pointerEnter.GetComponent<Selectable>();
            if (selectable == null)
            {
                selectable = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
            }
            EventSystem.current.SetSelectedGameObject(selectable.gameObject);
        }
    }

    protected virtual void OnPointerExit(BaseEventData eventData)
    {
        if (eventData.selectedObject == null)
        {
            return;
        }
        OnDeselect(eventData);
        EventSystem.current.SetSelectedGameObject(null);
    }

    protected virtual void OnNavigate(InputAction.CallbackContext context)
    {
        if (EventSystem.current.currentSelectedGameObject == null && _lastSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(_lastSelected.gameObject);
        }
    }
}
