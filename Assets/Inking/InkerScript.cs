using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkerScript : MonoBehaviour 
{
    public static InkerScript Instance { get; private set; }

    [SerializeField]
    private GameObject strokePrefab;

    [SerializeField]
    private bool doInk = false;
    public bool DoInk { get => doInk; set => doInk = value; }

    private bool lastDoInk;

    [SerializeField]
    private Transform inkTip;
    public Transform InkTip { get => inkTip; set => inkTip = value; }
    [SerializeField]
    private float segmentMinDist;
    [SerializeField]
    private float strokeWeight;

    private InkingStroke currentStroke;
    private Vector3 lastStrokePos;

    private void Awake()
    {
        Instance = this;
    }

    void Update () 
    {
        PlaceInkTip();
        if (DoInk)
        {
            if(!lastDoInk)
            {
                currentStroke = StartNewStroke();
            }


            Vector3 paintBrushMovement = inkTip.position - lastStrokePos;
            if (paintBrushMovement.magnitude > segmentMinDist)
            {
                currentStroke.AddPoint(inkTip.position, false);
                lastStrokePos = inkTip.position;
            }
        }
        if(lastDoInk && !DoInk)
        {
            currentStroke.AddPoint(inkTip.position, true);
            lastStrokePos = inkTip.position;
        }
        lastDoInk = DoInk;
    }

    private InkingStroke StartNewStroke()
    {
        GameObject retObj = Instantiate(strokePrefab);
        LineRenderer renderer = retObj.GetComponent<LineRenderer>();
        renderer.material.color = AnnotationColorManager.Instance.AnnotationColor;
        renderer.widthMultiplier = strokeWeight;
        InkingStroke ret = retObj.GetComponent<InkingStroke>();
        MainPrototypeScript.Instance.UndoStack.AddObject(retObj);
        return ret;
    }

    private void PlaceInkTip()
    {
        Plane inkingPlane = new Plane(Camera.main.transform.forward, AnnotationCursorBehavior.Instance.CenterDot.position);
        float enter;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (inkingPlane.Raycast(ray, out enter))
        {
            InkTip.position = ray.GetPoint(enter);
        }
    }
}
