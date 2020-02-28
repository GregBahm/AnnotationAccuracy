using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrototypeSelectionUi : MonoBehaviour
{
    [SerializeField]
    private GameObject selectionMain;

    [SerializeField]
    private Toggle existingToggle;

    [SerializeField]
    private Toggle snapToFeatureToggle;

    [SerializeField]
    private Toggle centerLockedToggle;

    [SerializeField]
    private GameObject visualizations;

    [SerializeField]
    private Toggle triangleToggle;

    [SerializeField]
    private Toggle ringToggle;

    [SerializeField]
    private Toggle meshToggle;

    [SerializeField]
    private Toggle lines;

    [SerializeField]
    private Toggle originalLayoutToggle;

    [SerializeField]
    private Toggle revisedLayoutToggle;

    public void ShowSelector()
    {
        selectionMain.SetActive(true);
    }

    public void HideSelector()
    {
        selectionMain.SetActive(false);
    }

    private void Update()
    {
        existingToggle.isOn = PrototypeConfiguration.Instance.Placement == PrototypeConfiguration.PlacementStyle.Legacy;
        snapToFeatureToggle.isOn = PrototypeConfiguration.Instance.Placement == PrototypeConfiguration.PlacementStyle.SnappingStyle;
        centerLockedToggle.isOn = PrototypeConfiguration.Instance.Placement == PrototypeConfiguration.PlacementStyle.Triangulated;

        visualizations.SetActive(PrototypeConfiguration.Instance.Placement == PrototypeConfiguration.PlacementStyle.Triangulated);

        triangleToggle.isOn = PrototypeConfiguration.Instance.SpatialEffects == PrototypeConfiguration.SpatialEffectsStyle.Triangle;
        ringToggle.isOn = PrototypeConfiguration.Instance.SpatialEffects == PrototypeConfiguration.SpatialEffectsStyle.Ring;
        meshToggle.isOn = PrototypeConfiguration.Instance.SpatialEffects == PrototypeConfiguration.SpatialEffectsStyle.Mesh;
        lines.isOn = PrototypeConfiguration.Instance.SpatialEffects == PrototypeConfiguration.SpatialEffectsStyle.LinesToTrianglePoint;

        originalLayoutToggle.isOn = PrototypeConfiguration.Instance.UiLayout == PrototypeConfiguration.UiLayoutStyle.Original;
        revisedLayoutToggle.isOn = PrototypeConfiguration.Instance.UiLayout == PrototypeConfiguration.UiLayoutStyle.Revised;
    }

    public void OnExistingToggled()
    {
        PrototypeConfiguration.Instance.Placement = PrototypeConfiguration.PlacementStyle.Legacy;
        PrototypeConfiguration.Instance.SpatialEffects = PrototypeConfiguration.SpatialEffectsStyle.None;
    }

    public void OnSnapToggled()
    {
        PrototypeConfiguration.Instance.Placement = PrototypeConfiguration.PlacementStyle.SnappingStyle;
        PrototypeConfiguration.Instance.SpatialEffects = PrototypeConfiguration.SpatialEffectsStyle.Snapping;
    }

    public void OnCenterLockedToggled()
    {
        PrototypeConfiguration.Instance.Placement = PrototypeConfiguration.PlacementStyle.Triangulated;
        if(PrototypeConfiguration.Instance.SpatialEffects == PrototypeConfiguration.SpatialEffectsStyle.Snapping
            || PrototypeConfiguration.Instance.SpatialEffects == PrototypeConfiguration.SpatialEffectsStyle.None)
        {
            PrototypeConfiguration.Instance.SpatialEffects = PrototypeConfiguration.SpatialEffectsStyle.Triangle;
        }
    }

    public void OnTriangleToggle()
    {
        PrototypeConfiguration.Instance.SpatialEffects = PrototypeConfiguration.SpatialEffectsStyle.Triangle;
    }
    public void OnRingToggle()
    {
        PrototypeConfiguration.Instance.SpatialEffects = PrototypeConfiguration.SpatialEffectsStyle.Ring;
    }
    public void OnMeshToggle()
    {
        PrototypeConfiguration.Instance.SpatialEffects = PrototypeConfiguration.SpatialEffectsStyle.Mesh;
    }
    public void OnLinesToggle()
    {
        PrototypeConfiguration.Instance.SpatialEffects = PrototypeConfiguration.SpatialEffectsStyle.LinesToTrianglePoint;
    }
    public void OnOriginalLayoutToggle()
    {
        PrototypeConfiguration.Instance.UiLayout = PrototypeConfiguration.UiLayoutStyle.Original;
    }
    public void OnRevisedLayoutToggle()
    {
        PrototypeConfiguration.Instance.UiLayout = PrototypeConfiguration.UiLayoutStyle.Revised;
    }
}
