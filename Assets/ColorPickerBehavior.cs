using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPickerBehavior : MonoBehaviour, IPointerClickHandler
{
    private int colorPickIndex;

    public Color[] Colors;

    [SerializeField]
    private TMP_Text colorPickerDot;
    [SerializeField]
    private Image circleFill;

    private void Start()
    {
        MainPrototypeScript.Instance.AnnotationColor = Colors[colorPickIndex];
    }

    private void Update()
    {
        colorPickerDot.color = MainPrototypeScript.Instance.AnnotationColor;
        circleFill.color = MainPrototypeScript.Instance.AnnotationColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        colorPickIndex = (colorPickIndex + 1) % Colors.Length;
        MainPrototypeScript.Instance.AnnotationColor = Colors[colorPickIndex];
    }
}
