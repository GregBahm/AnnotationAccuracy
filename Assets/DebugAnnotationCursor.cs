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

    public void DoUpdate(Vector3 positionTarget, Vector3 rayPos)
    {
        targetBall.position = positionTarget;
        Vector3 cylinderPos = (positionTarget + rayPos) / 2;
        cylinderPivot.position = cylinderPos;

        cylinderPivot.LookAt(rayPos);
        float dist = (positionTarget - rayPos).magnitude;
        dist = dist / cylinderPivot.parent.lossyScale.z;
        dist /= 2;
        cylinderPivot.localScale = new Vector3(1, 1, dist);
    }
}