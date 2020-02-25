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
    
    private Pose positionTarget = new Pose(Vector3.zero, Quaternion.identity);
    public Pose PositionTarget { get { return this.positionTarget; } }
    
    private void Awake()
    {
        Instance = this;
    }

    public void DoUpdate()
    {
        UpdateElementScale();

        //UpdatePositionTargetLegacyStyle();
        UpdatePositionTarget();
        UpdateVisualPositions();
        UpdateShaders();
    }

    private void UpdatePositionTarget()
    {
        Ray ray = GetCursorRay();

        float rayDist;
        TriangleMaker.Instance.TrianglePlane.Raycast(ray, out rayDist);
        Vector3 hitLocation = ray.origin + ray.direction * rayDist;
        Quaternion rotation = Quaternion.LookRotation(TriangleMaker.Instance.TrianglePlane.normal);
        positionTarget = new Pose(hitLocation, rotation);
    }

    private void UpdateShaders()
    {
        Shader.SetGlobalColor("_ShadowColor", AnnotationColorManager.Instance.AnnotationColor);
        Shader.SetGlobalVector("_CursorPos", centerDot.position);
    }

    private void UpdateVisualPositions()
    {
        SetCursorZ();
        ringPivot.rotation = PositionTarget.rotation;
    }

    private void SetCursorZ()
    {
        Vector3 targetInCameraSpace = Camera.main.transform.worldToLocalMatrix * new Vector4(positionTarget.position.x, positionTarget.position.y, positionTarget.position.z, 1);
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

    public static Vector3 NearestPointOnLine(Vector3 linePoint, Vector3 lineNormal, Vector3 searchPoint)
    {
        Vector3 unitVector = lineNormal.normalized;
        Vector3 offset = searchPoint - linePoint;
        float theDot = Vector3.Dot(offset, unitVector);
        return linePoint + unitVector * theDot;
    }

    private bool UpdatePositionTargetLegacyStyle()
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
