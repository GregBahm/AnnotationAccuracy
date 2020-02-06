using UnityEngine;
using UnityEngine.EventSystems;

public class PrototypeUiButton : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        MainPrototypeScript.Instance.OnMenuButtonPressed();
    }
}