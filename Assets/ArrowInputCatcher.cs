using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ArrowInputCatcher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private float clickDragDistThreshold;
    private Vector2 pressStartPos;

    private void Update()
    {
        if(ArrowController.Instance.IsRotating && !Input.GetMouseButton(0))
        {
            ArrowController.Instance.IsRotating = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MainPrototypeScript.Instance.Menus = MainPrototypeScript.MenuMode.None;
        ArrowController.Instance.IsRotating = true;
        pressStartPos = Input.mousePosition;
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
            || MainPrototypeScript.Instance.Tool != MainPrototypeScript.ToolMode.Arrows)
        {
            return false;
        }
        Vector2 mousePos = Input.mousePosition;
        float dragDist = (pressStartPos - mousePos).magnitude;
        return dragDist < clickDragDistThreshold;
    }
}
