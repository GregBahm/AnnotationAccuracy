using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackingManager : MonoBehaviour
{
    private Anchor theAnchor;

    private float trackingness;

    public bool HasTracking { get; private set; }

    [SerializeField]
    private Image trackingLossIndicator;

    [SerializeField]
    private Material ghostArrowMat;
    [SerializeField]
    private Material arrowRingMat;

    public static TrackingManager Instance { get; private set; }

    private Transform trackingBreadcrumb;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        trackingBreadcrumb = new GameObject("Tracking Breadcrumb").transform;
    }

    private void Update()
    {
        UpdateTrackingBreadcrumb();
        HasTracking = GetHasTracking();
        UpdateVisuals();
    }

    private void UpdateTrackingBreadcrumb()
    {
        if(TriangleMaker.Instance.FoundTriangle)
        {
            trackingBreadcrumb.position = AnnotationCursorBehavior.Instance.CenterDot.position;
        }
    }

    private void UpdateVisuals()
    {
        float trackingnessTarget = HasTracking ? 1 : 0;
        trackingness = Mathf.Lerp(trackingness, trackingnessTarget, Time.deltaTime * 4);

        float trackinglossAlpha = Mathf.Clamp01(1 - (trackingness * 2));
        trackingLossIndicator.color = new Color(1, 1, 1, trackinglossAlpha);

        float cursorElementAlpha = Mathf.Clamp01(trackingness * 2);
        ghostArrowMat.SetFloat("_Opacity", cursorElementAlpha * .6f);
        arrowRingMat.SetFloat("_Opacity", cursorElementAlpha);
    }

    private bool GetHasTracking()
    {
        if( TriangleMaker.Instance.FoundTriangle)
        {
            return true;
        }
        Vector3 viewPortPoint = Camera.main.WorldToViewportPoint(trackingBreadcrumb.position);
        return GetIsWithinCameraView(viewPortPoint);
    }

    private bool GetIsWithinCameraView(Vector3 viewPortPoint)
    {
        return viewPortPoint.z > 0
            && viewPortPoint.x > 0
            && viewPortPoint.y > 0
            && viewPortPoint.x < 1
            && viewPortPoint.y < 1;
    }

    public void ParentToAnchor(Transform obj)
    {
        if(theAnchor == null)
        {
            Pose anchorPose = new Pose(obj.position, obj.rotation);
            theAnchor = Session.CreateAnchor(anchorPose);
        }
        obj.SetParent(theAnchor.transform, true);
    }
}