using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Marvrus.UI;
using UnityEngine.UI;

public class DragExit : UIBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public enum DragType
    {
        Top_Down,
        Bottom_Up,
        LeftToRight,
        RightToLeft
    }

    public DragType dragType = DragType.Top_Down;

    private UIPopup popup;
    private Action exitEvent;
    private Vector2 start, end;
    private bool isSuccess = false;

    protected override void Awake()
    {
        base.Awake();
 
        if (popup == null)
            popup = gameObject.GetComponent<UIPopup>();
    }

    public void SetExitEvet(Action _callback)
    {
        exitEvent = _callback;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        start = eventData.position;
        isSuccess = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        end = eventData.position;

        switch (dragType)
        {
            case DragType.Top_Down:
                isSuccess = (start.y - end.y) > Const.SWIPE_DRAG_SIZE;
                break;

            case DragType.Bottom_Up:
                isSuccess = (end.y - start.y) > Const.SWIPE_DRAG_SIZE;
                break;

            case DragType.LeftToRight:
                isSuccess = (end.x - start.x) > Const.SWIPE_DRAG_SIZE;
                break;

            case DragType.RightToLeft:
                isSuccess = (start.x - end.x) > Const.SWIPE_DRAG_SIZE;
                break;
        }

        if (isSuccess is false)
            return;

        if (popup is null)
        {
            exitEvent?.Invoke();
        }
        else
        {
            popup.Hide();
        }
    }
}
