using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PrototypeMenuButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private MainPrototypeScript.MenuMode mode;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if(MainPrototypeScript.Instance.Menus == mode)
        {
            MainPrototypeScript.Instance.Menus = MainPrototypeScript.MenuMode.None;
        }
        else
        {
            MainPrototypeScript.Instance.Menus = mode;
        }
    }
}
