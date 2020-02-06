using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddAnnotationButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private MainPrototypeScript main;

    public void OnPointerDown(PointerEventData eventData)
    {
        //this.main.OnBackgroundPressed();
    }
}