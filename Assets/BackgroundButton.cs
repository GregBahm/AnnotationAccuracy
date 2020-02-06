using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        MainPrototypeScript.Instance.Menus = MainPrototypeScript.MenuMode.None;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //MainPrototypeScript.Instance.OnBackgroundPressed();
    }
}
