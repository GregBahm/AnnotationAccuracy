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
        MainPrototypeScript.Instance.Menus = MainPrototypeScript.MenuMode.None;
        if (MainPrototypeScript.Instance.Tool == MainPrototypeScript.ToolMode.Inking)
        {
            InkerScript.Instance.DoInk = true;
        }
    }
}
