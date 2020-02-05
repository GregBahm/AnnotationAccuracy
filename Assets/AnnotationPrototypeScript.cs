using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationPrototypeScript : MonoBehaviour
{
    [SerializeField]
    private PrototypeMenuButton CallMenu;
    [SerializeField]
    private PrototypeMenuButton AnnotationMenu;

    [SerializeField]
    private float addArrowRotationSensitivity;

    [SerializeField]
    private ToolMode mode;
    [SerializeField]
    private InkerScript inking;
    public ToolMode Mode { get => mode; set => mode = value; }

    private AnnotationCursorBehavior cursor;

    private bool annotationPressed;
    private Vector2 startArrowRotationVector;
    private Vector2 annotationPressPos;

    private List<GameObject> annotationsList = new List<GameObject>();
    
    public enum ToolMode
    {
        Inking,
        Arrows,
        None
    }

    private void Start()
    {
        cursor = GetComponent<AnnotationCursorBehavior>();
    }

    public void OnCallMenuPressed()
    {
        CallMenu.PopoutShown = !CallMenu.PopoutShown;
        if(CallMenu.PopoutShown)
        {
            AnnotationMenu.PopoutShown = false;
        }
    }

    public void OnAnnotationMenuPressed()
    {
        AnnotationMenu.PopoutShown = !AnnotationMenu.PopoutShown;
        if (AnnotationMenu.PopoutShown)
        {
            CallMenu.PopoutShown = false;
        }
    }

    public void OnAddAnnotationPressed()
    {
        annotationPressed = true;
        annotationPressPos = Input.mousePosition;
        startArrowRotationVector = this.cursor.ArrowRotationVector / this.addArrowRotationSensitivity;
    }

    private void Update()
    {
        UpdateArrows();
        UpdateInking();
    }

    private void UpdateInking()
    {
        if(mode == ToolMode.Inking)
        {
            inking.DoInk = Input.GetMouseButton(0); // TODO: make sure this isn't touching a UI button
        }
        PlaceInkTip();
    }

    private void PlaceInkTip()
    {
        Plane inkingPlane = new Plane(Camera.main.transform.forward, cursor.CenterDot.position);
        float enter;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(inkingPlane.Raycast(ray, out enter))
        {
            inking.InkTip.position = ray.GetPoint(enter);
        }
    }

    private void UpdateArrows()
    {
        cursor.Arrow.gameObject.SetActive(mode == ToolMode.Arrows);
        if (annotationPressed && mode == ToolMode.Arrows)
        {
            if (Input.GetMouseButton(0))
            {
                PreparePlacement();
            }
            else
            {
                annotationPressed = false;
                CommitPlacement();
            }
        }
    }

    private void CommitPlacement()
    {
        GameObject newArrow = Instantiate(this.cursor.Arrow.gameObject);
        newArrow.transform.position = this.cursor.Arrow.position;
        newArrow.transform.rotation = this.cursor.Arrow.rotation;
        newArrow.transform.localScale = this.cursor.Arrow.lossyScale;
        annotationsList.Add(newArrow);
    }

    private void PreparePlacement()
    {
        Vector2 currentMousePos = Input.mousePosition;
        Vector2 drag = this.annotationPressPos - currentMousePos;
        Vector2 newArrowVector = this.startArrowRotationVector + new Vector2(-drag.x, drag.y);
        newArrowVector *= addArrowRotationSensitivity;
        if(newArrowVector.magnitude > 1)
        {
            newArrowVector = newArrowVector.normalized;
        }
        float zVal = 1 - newArrowVector.magnitude;
        this.cursor.ArrowRotationVector = new Vector3(newArrowVector.x, newArrowVector.y, -zVal).normalized;
    }
}
