using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappingCursor : MonoBehaviour
{
    public static SnappingCursor Instance { get; private set; }

    [SerializeField]
    private Transform main;
    public Transform Main { get { return main; } }

    [SerializeField]
    private LineRenderer line;

    private Material lineMat;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        lineMat = line.sharedMaterial;
    }

    public void DoUpdate()
    {
        Vector3 cursorPos = AnnotationCursorBehavior.Instance.CenterDot.position;
        SetZPosition(cursorPos);
        SetLine(cursorPos);
    }
    
    private void SetZPosition(Vector3 cursorPos)
    {
        Vector3 targetInCameraSpace = Camera.main.transform.worldToLocalMatrix * new Vector4(cursorPos.x, cursorPos.y, cursorPos.z, 1);
        main.transform.localPosition = new Vector3(0, 0, targetInCameraSpace.z);
    }

    private void SetLine(Vector3 cursorPos)
    {
        Vector3 localPos = ToLocalSpace(cursorPos);
        lineMat.SetFloat("_LineLength", localPos.magnitude);
        line.SetPosition(1, localPos);
    }

    private Vector3 ToLocalSpace(Vector3 worldPoint)
    {
        return main.transform.worldToLocalMatrix * new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, 1);
    }
}
