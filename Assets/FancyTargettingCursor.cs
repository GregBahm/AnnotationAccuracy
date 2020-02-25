using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FancyTargettingCursor : MonoBehaviour
{
    public static FancyTargettingCursor Instance { get; private set; }

    [SerializeField]
    private float ringSize;
    [SerializeField]
    private Material lineSegementMat;

    [SerializeField]
    private LineRenderer circleSegmentA;
    [SerializeField]
    private LineRenderer circleSegmentB;
    [SerializeField]
    private LineRenderer circleSegmentC;
    [SerializeField]
    private LineRenderer lineSegmentA;
    [SerializeField]
    private LineRenderer lineSegmentB;
    [SerializeField]
    private LineRenderer lineSegmentC;

    [SerializeField]
    private float lineThickness = 0.01f;
    [SerializeField]
    private int circleSegmentPoints = 32;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        lineSegmentA.material = new Material(lineSegementMat);
        lineSegmentB.material = new Material(lineSegementMat);
        lineSegmentC.material = new Material(lineSegementMat);
    }

    public void DoUpdate()
    {
        UpdateLineWidths();
        UpdateLinePositions();
    }

    private void UpdateLineWidths()
    {
        circleSegmentA.widthMultiplier = lineThickness;
        circleSegmentB.widthMultiplier = lineThickness;
        circleSegmentC.widthMultiplier = lineThickness;
        lineSegmentA.widthMultiplier = lineThickness;
        lineSegmentB.widthMultiplier = lineThickness;
        lineSegmentC.widthMultiplier = lineThickness;
    }

    private void UpdateLinePositions()
    {
        SetSegment(lineSegmentA, TriangleMaker.Instance.PointA);
        SetSegment(lineSegmentB, TriangleMaker.Instance.PointB);
        SetSegment(lineSegmentC, TriangleMaker.Instance.PointC);
    }

    private void SetSegment(LineRenderer lineSegment, Vector3 worldTriPoint)
    {
        Vector3 localPos = ToLocalSpace(worldTriPoint);
        lineSegment.SetPosition(1, localPos);

        float materialParam = localPos.magnitude;
        lineSegment.material.SetFloat("_SegmentLength", materialParam);
        lineSegment.material.SetFloat("_RingSize", ringSize);
    }

    private Vector3 ToLocalSpace(Vector3 worldPoint)
    {
        return transform.worldToLocalMatrix * new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, 1);
    }
}
