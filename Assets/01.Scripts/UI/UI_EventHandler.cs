using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Action OnClickAction = null;
    public Action OnDragAction = null;
    public Action OnDownAction = null;
    public Action OnUpAction = null;
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnDragHandler = null;
    public Action<PointerEventData> OnDownHandler = null;
    public Action<PointerEventData> OnUpHandler = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler != null)
            OnClickHandler.Invoke(eventData);

        OnClickAction?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);

        OnDragAction?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (OnDownHandler != null)
            OnDownHandler.Invoke(eventData);

        OnDownAction?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (OnUpHandler != null)
            OnUpHandler.Invoke(eventData);

        OnUpAction?.Invoke();
    }
}