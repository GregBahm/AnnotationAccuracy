using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ArrowInputCatcher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private float clickDragDistThreshold;

    [SerializeField]
    private Material ghostArrowMat;

    private Vector2 pressStartPos;
    private bool isPressed;

    private void Update()
    {
        if(PrototypeConfiguration.Instance.Placement == PrototypeConfiguration.PlacementStyle.Legacy)
        {
            DoLegacyPlacement();
        }
        else
        {
            DoCenterLockedStyleUpdate();
        }
    }

    private void DoLegacyPlacement()
    {
        if(!Input.GetMouseButton(0))
        {
            if(isPressed)
            {
                ArrowController.Instance.AddArrow();
            }
            isPressed = false;
        }

        ghostArrowMat.SetFloat("_OpacityRange", isPressed ? .6f : 0);
    }

    private void DoCenterLockedStyleUpdate()
    {
        if (ArrowController.Instance.IsRotating && !Input.GetMouseButton(0))
        {
            ArrowController.Instance.IsRotating = false;
        }
        ghostArrowMat.SetFloat("_OpacityRange", .6f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MainUiScript.Instance.Menus = MainUiScript.MenuMode.None;
        ArrowController.Instance.IsRotating = true;
        pressStartPos = Input.mousePosition;
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(GetShouldAddArrow())
        {
            ArrowController.Instance.AddArrow();
        }
    }
    private bool GetShouldAddArrow()
    {
        if(!ArrowController.Instance.IsRotating
            || MainUiScript.Instance.Tool != MainUiScript.ToolMode.Arrows)
        {
            return false;
        }
        Vector2 mousePos = Input.mousePosition;
        float dragDist = (pressStartPos - mousePos).magnitude;
        return dragDist < clickDragDistThreshold;
    }
}
