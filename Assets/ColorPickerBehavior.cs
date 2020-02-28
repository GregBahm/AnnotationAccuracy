using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPickerBehavior : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private TMP_Text colorPickerDot;
    [SerializeField]
    private Image circleFill;
    
    private void Update()
    {
        colorPickerDot.color = AnnotationColorManager.Instance.AnnotationColor;
        if(circleFill != null)
        {
            circleFill.color = AnnotationColorManager.Instance.AnnotationColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AnnotationColorManager.Instance.GoToNextColor();
    }
}
