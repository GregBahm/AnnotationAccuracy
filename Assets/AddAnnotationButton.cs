using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddAnnotationButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private AnnotationPrototypeScript main;

    public void OnPointerDown(PointerEventData eventData)
    {
        this.main.OnAddAnnotationPressed();
    }
}