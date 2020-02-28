using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolSwitcherButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    private Vector3 pressedPosition;
    private bool pressed;
    private bool dragging;

    [SerializeField]
    private MainUiScript.ToolMode tool;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!dragging)
        {
            MainUiScript.Instance.Tool = tool;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //TODO: Work this out
    }
}
