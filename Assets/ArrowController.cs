using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public static ArrowController Instance { get; private set; }

    [SerializeField]
    private Transform arrowPivot;
    [SerializeField]
    private Transform arrowEnd;
    [SerializeField]
    private float rotationSensitivity;
    [Range(0, 1)]
    [SerializeField]
    private float minimumZ;

    [SerializeField]
    private Material ghostArrowMat;
    [SerializeField]
    private Material placedArrowMat;

    private bool wasRotating;
    private Vector3 startArrowVector;
    private Vector2 startInputLocation;

    public bool IsRotating { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(MainPrototypeScript.Instance.Tool != MainPrototypeScript.ToolMode.Arrows)
        {
            arrowPivot.gameObject.SetActive(false);
        }
        else
        {
            arrowPivot.gameObject.SetActive(true);
            UpdateColors();
            if (IsRotating)
            {
                if (!wasRotating)
                {
                    StartRotation();
                }
                UpdateRotation();
            }

            wasRotating = IsRotating;
        }
    }

    private void UpdateColors()
    {
        ghostArrowMat.SetColor("_Color", MainPrototypeScript.Instance.AnnotationColor);
        placedArrowMat.SetColor("_Color", MainPrototypeScript.Instance.AnnotationColor);
    }

    private void StartRotation()
    {
        startInputLocation = Input.mousePosition;
        startArrowVector = GetStartArrowVector();
    }

    private Vector3 GetStartArrowVector()
    {
        return Camera.main.transform.worldToLocalMatrix * arrowPivot.forward;
    }

    private void UpdateRotation()
    {
        Vector2 currentMousePos = Input.mousePosition;
        Vector2 drag = this.startInputLocation - currentMousePos;
        drag *= rotationSensitivity;
        Vector3 newArrowVector = this.startArrowVector + new Vector3(drag.x, drag.y, 0);
        Vector3 arrowForward = GetArrowForward(newArrowVector);
        Vector3 arrowUp = Vector3.Cross(arrowForward, Vector3.forward);
        Quaternion arrowRotation = Quaternion.LookRotation(arrowForward, arrowUp);
        this.arrowPivot.localRotation = arrowRotation;
    }

    private Vector3 GetArrowForward(Vector3 newArrowVector)
    {
        Vector3 ret = newArrowVector.normalized;
        if(ret.z < minimumZ)
        {
            return new Vector3(ret.x, ret.y, minimumZ).normalized;
        }
        return ret;
    }


    public void AddArrow()
    {
        GameObject newArrow = Instantiate(this.arrowPivot.gameObject);
        newArrow.transform.position = this.arrowPivot.position;
        newArrow.transform.rotation = this.arrowPivot.rotation;
        newArrow.transform.localScale = this.arrowPivot.lossyScale;
        Material newMat = new Material(placedArrowMat);
        newArrow.GetComponentInChildren<MeshRenderer>().sharedMaterial = newMat;

        MainPrototypeScript.Instance.UndoStack.AddObject(newArrow);
    }
}
