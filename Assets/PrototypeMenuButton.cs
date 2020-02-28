using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PrototypeMenuButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private MainUiScript.MenuMode mode;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if(MainUiScript.Instance.Menus == mode)
        {
            MainUiScript.Instance.Menus = MainUiScript.MenuMode.None;
        }
        else
        {
            MainUiScript.Instance.Menus = mode;
        }
    }
}
