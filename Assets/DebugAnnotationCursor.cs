using UnityEngine;

public class DebugAnnotationCursor : MonoBehaviour
{
    public static DebugAnnotationCursor Instance { get; private set; }

    [SerializeField]
    private float cylinderScale;

    [SerializeField]
    private Transform cylinderPivot;

    private void Awake()
    {
        Instance = this;
    }

    public void DoUpdate()
    {
        DoUpdate(AnnotationCursorBehavior.Instance.PositionTarget.position,
            AnnotationCursorBehavior.Instance.DebugRayStart);
    }

    private void DoUpdate(Vector3 positionTarget, Vector3 rayPos)
    {
        Vector3 cylinderPos = (positionTarget + rayPos) / 2;
        cylinderPivot.position = cylinderPos;

        cylinderPivot.LookAt(rayPos);
        float dist = (positionTarget - rayPos).magnitude;
        dist /= 2;
        cylinderPivot.localScale = new Vector3(cylinderScale, cylinderScale, dist);
    }
}