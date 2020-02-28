using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainUiScript : MonoBehaviour
{
    public static MainUiScript Instance { get; private set; }

    [SerializeField]
    private GameObject toolsMenu;
    [SerializeField]
    private GameObject callMenu;
    [SerializeField]
    private InkerScript inking;

    [SerializeField]
    private GameObject originalUi;
    [SerializeField]
    private GameObject revisedUi;

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

    public void DoUpdate(PrototypeConfiguration.UiLayoutStyle layoutStyle)
    {
        originalUi.SetActive(layoutStyle == PrototypeConfiguration.UiLayoutStyle.Original);
        revisedUi.SetActive(layoutStyle == PrototypeConfiguration.UiLayoutStyle.Revised);
        UpdateMenus();
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
