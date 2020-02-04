using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnotationCursorBehavior : MonoBehaviour
{
    [SerializeField]
    private Transform root;
    [SerializeField]
    private Transform centerDot;
    [SerializeField]
    private Transform ring;
    [SerializeField]
    private Transform arrowPivot;
    [SerializeField]
    private Transform arrow;
    public Transform Arrow {get{return this.arrow;}}

    [SerializeField]
    [Range(0, 10)]
    private float visualsScale = 1;
    [SerializeField]
    public float positionSmoothing = 10;
    [SerializeField]
    public float rotationSmoothing = 10;


    public Vector3 ArrowRotationVector;

    private Pose positionTarget = new Pose(Vector3.zero, Quaternion.identity);

    public bool TargetFound { get; private set; }
    
    private void Update()
    {
        UpdateElementScale();
        TargetFound = UpdateTargetPos();
        UpdateVisualPositions();
        UpdateShaders();
    }

    private void UpdateShaders()
    {
        Shader.SetGlobalVector("_CursorPos", centerDot.position);
    }

    private void UpdateVisualPositions()
    {
        centerDot.LookAt(Camera.main.transform);
        root.position = Vector3.Lerp(root.position, positionTarget.position, Time.deltaTime * positionSmoothing);
        ring.rotation = Quaternion.Lerp(ring.rotation, positionTarget.rotation, Time.deltaTime * rotationSmoothing);
        arrowPivot.localRotation = Quaternion.LookRotation(ArrowRotationVector);
    }

    private void UpdateElementScale()
    {
        root.localScale = new Vector3(visualsScale, visualsScale, visualsScale);
    }

    private static readonly TrackableHitFlags priorityFlags =
        TrackableHitFlags.FeaturePoint |
        TrackableHitFlags.FeaturePointWithSurfaceNormal |
        TrackableHitFlags.PlaneWithinBounds;

    private bool UpdateTargetPos()
    {
        float x = Camera.main.pixelWidth / 2;
        float y = Camera.main.pixelHeight / 2;
        TrackableHit hit;
        if (Frame.Raycast(x, y, priorityFlags, out hit))
        {
            positionTarget = hit.Pose;
            return true;
        }
        return false;
    }
}
