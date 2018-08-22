using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent OnLeftClick;
    public UnityEvent OnRightClick;
    public UnityEvent OnMiddleMouseClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch(eventData.button)
        {
            case PointerEventData.InputButton.Left:
                if(OnLeftClick != null)
                {
                    OnLeftClick.Invoke();
                }
                break;
            case PointerEventData.InputButton.Right:
                if (OnRightClick != null)
                {
                    OnRightClick.Invoke();
                }
                break;
            case PointerEventData.InputButton.Middle:
                if (OnMiddleMouseClick != null)
                {
                    OnMiddleMouseClick.Invoke();
                }
                break;
        }
    }
}
