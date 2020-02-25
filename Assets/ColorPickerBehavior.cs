using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPickerBehavior : MonoBehaviour, IPointerClickHandler
{
    private int colorPickIndex;

    [SerializeField]
    private TMP_Text colorPickerDot;
    [SerializeField]
    private Image circleFill;

    private void Start()
    {
        AnnotationColorManager.Instance.AnnotationColor = AnnotationColorManager.Instance.Colors[colorPickIndex];
    }

    private void Update()
    {
        colorPickerDot.color = AnnotationColorManager.Instance.AnnotationColor;
        circleFill.color = AnnotationColorManager.Instance.AnnotationColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        colorPickIndex = (colorPickIndex + 1) % AnnotationColorManager.Instance.Colors.Length;
        AnnotationColorManager.Instance.AnnotationColor = AnnotationColorManager.Instance.Colors[colorPickIndex];
    }
}
