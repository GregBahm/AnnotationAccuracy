using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeConfiguration : MonoBehaviour
{
    [SerializeField]
    private PlacementStyle placement;

    [SerializeField]
    private SpatialEffectsStyle spatialEffects;

    [SerializeField]
    private UiLayoutStyle uiLayout;
    
    public enum UiLayoutStyle
    {
        Original,
        Revised,
    }

    public enum PlacementStyle
    {
        Triangulated,
        Legacy,
        SnappingStyle
    }

    public enum SpatialEffectsStyle
    {
        Ring,
        LinesToTrianglePoint,
        Triangle,
        Mesh,
        Snapping,
        //PulsingPoints
    }
    
    private void Update()
    {
        DoMeshUpdate();
        DoCursorUpdate();
        DoSnapingCursorUpdate();
        UpdateLinesToTriangleCursor();
        UpdateUi();
    }

    private void UpdateUi()
    {
        MainUiScript.Instance.DoUpdate(uiLayout);
    }

    private void DoSnapingCursorUpdate()
    {
        bool showIt = spatialEffects == SpatialEffectsStyle.Snapping;
        SnappingCursor.Instance.Main.gameObject.SetActive(showIt);
        if(showIt)
        {
            SnappingCursor.Instance.DoUpdate();
        }
    }

    private void UpdateLinesToTriangleCursor()
    {
        bool showIt = spatialEffects == SpatialEffectsStyle.LinesToTrianglePoint;
        FancyTargettingCursor.Instance.gameObject.SetActive(showIt);
        if(showIt)
        {
            FancyTargettingCursor.Instance.DoUpdate();
        }
    }

    private void DoCursorUpdate()
    {
        bool showTriangle = spatialEffects == SpatialEffectsStyle.Triangle;
        TriangleMaker.Instance.DoUpdate(showTriangle);

        bool showRing = spatialEffects == SpatialEffectsStyle.Ring;
        AnnotationCursorBehavior.Instance.DoUpdate(placement, showRing);
    }

    private void DoMeshUpdate()
    {
        bool doMesh = spatialEffects == SpatialEffectsStyle.Mesh;
        MeshBuilder.Instance.enabled = doMesh;

        if (doMesh)
        {
            MeshBuilder.Instance.BuildMeshFromFeaturePoints();
        }
    }
}
