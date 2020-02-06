using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPrototypeScript : MonoBehaviour
{
    public static MainPrototypeScript Instance { get; private set; }

    public Color AnnotationColor { get; set; }

    [SerializeField]
    private GameObject toolsMenu;
    [SerializeField]
    private GameObject callMenu;

    [SerializeField]
    private ToolMode tool;
    [SerializeField]
    private InkerScript inking;
    public ToolMode Tool { get => tool; set => tool = value; }

    private AnnotationCursorBehavior cursor;

    private bool mousePressingButton;
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

    private void Start()
    {
        cursor = GetComponent<AnnotationCursorBehavior>();
    }

    private void Update()
    {
        UpdateMenus();
        UpdateInking();
    }

    public void OnMenuButtonPressed()
    {
        mousePressingButton = true;
    }

    private void UpdateMenus()
    {
        toolsMenu.SetActive(Menus == MenuMode.Tools);
        callMenu.SetActive(Menus == MenuMode.Call); 
    }

    private void UpdateInking()
    {
        if(tool == ToolMode.Inking)
        {
            inking.DoInk = Input.GetMouseButton(0); // TODO: make sure this isn't touching a UI button
        }
        else
        {
            inking.DoInk = false;
        }
        PlaceInkTip();
    }

    private void PlaceInkTip()
    {
        Plane inkingPlane = new Plane(Camera.main.transform.forward, cursor.CenterDot.position);
        float enter;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(inkingPlane.Raycast(ray, out enter))
        {
            inking.InkTip.position = ray.GetPoint(enter);
        }
    }
}
