using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnotationCursorBehavior : MonoBehaviour
{
    public static AnnotationCursorBehavior Instance { get; private set; }

    [SerializeField]
    private Transform root;
    [SerializeField]
    private Transform centerDot;
    public Transform CenterDot { get => centerDot; set => centerDot = value; }
    [SerializeField]
    private Transform ringPivot;
    [SerializeField]
    private Transform ring;

    [SerializeField]
    [Range(0, 10)]
    private float visualsScale = 1;
    [SerializeField]
    public float positionSmoothing = 10;
    [SerializeField]
    public float rotationSmoothing = 10;

    [SerializeField]
    private float minZ;
    [SerializeField]
    private float maxZ;

    [SerializeField]
    private Material ringMat;

    private Pose positionTarget = new Pose(Vector3.zero, Quaternion.identity);
    public Pose PositionTarget { get { return this.positionTarget; } }

    private float targetFoundNess;
    public bool TargetFound { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void DoUpdate()
    {
        UpdateElementScale();
        //TargetFound = UpdateTargetPos();
        //if(!TargetFound)
        //{
            //DeepSearchTargetPos();
            DeepSearchScreenspaceStyle();
        //}
        UpdateVisualPositions();
        UpdateShaders();
    }

    private void UpdateShaders()
    {
        Shader.SetGlobalColor("_ShadowColor", MainPrototypeScript.Instance.AnnotationColor);
        Shader.SetGlobalVector("_CursorPos", centerDot.position);

        float cursorAlphaTarget = TargetFound ? 1 : 0;
        targetFoundNess = Mathf.Lerp(targetFoundNess, cursorAlphaTarget, Time.deltaTime * 4);
        ringMat.SetFloat("_Opacity", targetFoundNess);
    }

    private void UpdateVisualPositions()
    {
        SetCursorZ();
        ringPivot.rotation = Quaternion.Lerp(ringPivot.rotation, positionTarget.rotation, Time.deltaTime * rotationSmoothing);
        float ringScale = Mathf.Lerp(2, 1, targetFoundNess);
        ring.localScale = new Vector3(ringScale, ringScale, 1);
    }

    private void SetCursorZ()
    {
        Vector3 targetInCameraSpace = Camera.main.transform.worldToLocalMatrix * new Vector4(positionTarget.position.x, positionTarget.position.y, positionTarget.position.z, 1);
        //root.localPosition = new Vector3(0, 0, targetInCameraSpace.z);

        float zTarget = Mathf.Clamp(targetInCameraSpace.z, minZ, maxZ);
        Vector3 localTarget = new Vector3(0, 0, zTarget);
        root.localPosition = Vector3.Lerp(root.localPosition, localTarget, Time.deltaTime * positionSmoothing);
    }

    private void UpdateElementScale()
    {
        root.localScale = new Vector3(visualsScale, visualsScale, visualsScale);
    }

    private static readonly TrackableHitFlags priorityFlags =
        TrackableHitFlags.FeaturePoint |
        TrackableHitFlags.FeaturePointWithSurfaceNormal |
        TrackableHitFlags.PlaneWithinBounds;

    private Ray GetCursorRay()
    {
        return new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        float x = Camera.main.pixelWidth / 2;
        float y = Camera.main.pixelHeight / 2;
        return Camera.main.ScreenPointToRay(new Vector3(x, y, 0));
    }

    private void DeepSearchTargetPos()
     {
        float minDist = Mathf.Infinity;

        Ray cursorRay = GetCursorRay();
        for (int i = 0; i < Frame.PointCloud.PointCount; i++)
        {
            PointCloudPoint point = Frame.PointCloud.GetPointAsStruct(i);
            Vector3 onRay = NearestPointOnLine(cursorRay.origin, cursorRay.direction, point.Position);
            float distToRay = (onRay - point.Position).magnitude;
            if (distToRay < minDist)
            {
                minDist = distToRay;
                Quaternion rotation = Quaternion.identity; // Quaternion.LookRotation(rayToPoint);
                positionTarget = new Pose(point.Position, rotation);
                DebugRayStart = onRay;
                TargetFound = true;
            }
        }
    }

    private void DeepSearchScreenspaceStyle()
    {
        float minDist = Mathf.Infinity;

        Ray cursorRay = GetCursorRay();
        for (int i = 0; i < Frame.PointCloud.PointCount; i++)
        {
            PointCloudPoint point = Frame.PointCloud.GetPointAsStruct(i);
            float distToScreenpoint = GetScreenpointDistance(point.Position);
            if (distToScreenpoint < minDist)
            {
                minDist = distToScreenpoint;
                Quaternion rotation = Quaternion.identity; // Quaternion.LookRotation(rayToPoint);
                positionTarget = new Pose(point.Position, rotation);
                DebugRayStart = CenterDot.position;
                TargetFound = true;
            }
        }
    }

    private float GetScreenpointDistance(Vector3 position)
    {
        Vector3 screenPointInPixels = Camera.main.WorldToScreenPoint(position);
        float xParam = screenPointInPixels.x - Camera.main.pixelWidth / 2;
        float yParam = screenPointInPixels.y - Camera.main.pixelHeight / 2;
        return new Vector2(xParam, yParam).magnitude;
    }

    public static Vector3 NearestPointOnLine(Vector3 linePoint, Vector3 lineNormal, Vector3 searchPoint)
    {
        Vector3 unitVector = lineNormal.normalized;
        Vector3 offset = searchPoint - linePoint;
        float theDot = Vector3.Dot(offset, unitVector);
        return linePoint + unitVector * theDot;
    }

    public Vector3 DebugRayStart { get; private set; }

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
