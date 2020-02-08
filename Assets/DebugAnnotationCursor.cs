using UnityEngine;

public class DebugAnnotationCursor : MonoBehaviour
{
    public static DebugAnnotationCursor Instance { get; private set; }

    [SerializeField]
    private Transform targetBall;

    [SerializeField]
    private Transform cylinderPivot;

    private void Awake()
    {
        Instance = this;
    }

    public void DoUpdate()
    {
        Vector3 positionTarget = AnnotationCursorBehavior.Instance.PositionTarget.position;
        Vector3 centerDotPos = AnnotationCursorBehavior.Instance.CenterDot.position;
        targetBall.position = positionTarget;
        Vector3 cylinderPos = (positionTarget + centerDotPos) / 2;
        cylinderPivot.position = cylinderPos;

        cylinderPivot.LookAt(centerDotPos);
        float dist = (positionTarget - centerDotPos).magnitude;
        dist = dist / cylinderPivot.parent.lossyScale.z;
        dist /= 2;
        cylinderPivot.localScale = new Vector3(1, 1, dist);
    }
}