using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorDistanceDisplayer : MonoBehaviour
{
    [SerializeField]
    private Transform plane;
    [SerializeField]
    private Material planeMat;

    public static CursorDistanceDisplayer Instance;

    private Vector3 positionTarget;
    private float planeScaleTarget;

    private void Awake()
    {
        Instance = this;
    }

    public void DoUpdate()
    {
        Vector3 cursorPos = AnnotationCursorBehavior.Instance.CenterDot.position;
        Vector3 cameraPos = Camera.main.transform.position;

        positionTarget = new Vector3(cameraPos.x, cursorPos.y, cameraPos.z);
        planeScaleTarget = (cursorPos - positionTarget).magnitude;
        plane.transform.position = Vector3.Lerp(plane.transform.position, positionTarget, Time.deltaTime * 1);

        float planeScale = Mathf.Lerp(plane.transform.localScale.x, planeScaleTarget, Time.deltaTime * 10);
        plane.transform.localScale = new Vector3(planeScale, 1, planeScale);
    }
}