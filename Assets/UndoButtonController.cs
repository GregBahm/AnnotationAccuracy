using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UndoButtonController : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if(MainUiScript.Instance.UndoStack.CanUndo)
        {
            MainUiScript.Instance.UndoStack.Undo();
        }
    }
}
