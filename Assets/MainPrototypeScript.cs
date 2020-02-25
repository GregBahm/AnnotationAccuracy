using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainPrototypeScript : MonoBehaviour
{
    public static MainPrototypeScript Instance { get; private set; }

    [SerializeField]
    private GameObject toolsMenu;
    [SerializeField]
    private GameObject callMenu;
    [SerializeField]
    private InkerScript inking;

    [SerializeField]
    private ToolMode tool;
    public ToolMode Tool { get => tool; set => tool = value; }
    
    public AnnotationsUndoStack UndoStack { get; } = new AnnotationsUndoStack();
    
    public MenuMode Menus { get; set; } = MenuMode.None;
    
    public enum ToolMode
    {
        Inking,
        Arrows
    }

    public enum MenuMode
    {
        Tools,
        Call,
        None
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdateMenus();
        UpdateCursor();
        MeshBuilder.Instance.BuildMeshFromFeaturePoints();
        FancyTargettingCursor.Instance.DoUpdate();
    }

    private void UpdateCursor()
    {
        AnnotationCursorBehavior.Instance.DoUpdate();
        TriangleMaker.Instance.DoUpdate();
    }

    public void OnMenuButtonPressed()
    {
    }

    private void UpdateMenus()
    {
        toolsMenu.SetActive(Menus == MenuMode.Tools);
        callMenu.SetActive(Menus == MenuMode.Call); 
    }
}
