using UnityEngine;
using UnityEngine.EventSystems;

public class InkInputCatcher : MonoBehaviour, IPointerDownHandler
{
    void Update()
    {
        if(InkerScript.Instance.DoInk && !Input.GetMouseButton(0))
        {
            InkerScript.Instance.DoInk = false;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        MainUiScript.Instance.Menus = MainUiScript.MenuMode.None;
        if (MainUiScript.Instance.Tool == MainUiScript.ToolMode.Inking)
        {
            InkerScript.Instance.DoInk = true;
        }
    }
}
