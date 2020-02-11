using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepSearchTestScript : MonoBehaviour
{
    public Transform[] FakePoints;

    public Transform FakeCamera;

    public Transform cylinderPivot;
    public Transform TargetBall;

    private void Update()
    {
        DeepSearchTargetPos();
        PlaceDebugCylinder(RayStart, RayEnd);
    }
    public Vector3 RayStart { get; private set; }
    public Vector3 RayEnd { get; private set; }

    private void DeepSearchTargetPos()
    {
        float minDist = Mathf.Infinity;
        
        for (int i = 0; i < FakePoints.Length; i++)
        {
            Vector3 pos = FakePoints[i].position;
            Vector3 onRay = NearestPointOnLine(FakeCamera.position, FakeCamera.forward, pos);

            float distToRay = (onRay - pos).magnitude;
            if (distToRay < minDist)
            {
                minDist = distToRay;
                RayEnd = pos;
                RayStart = onRay;
            }
        }
    }
    public static Vector3 NearestPointOnLine(Vector3 linePoint, Vector3 lineNormal, Vector3 searchPoint)
    {
        Vector3 unitVector = lineNormal.normalized;
        Vector3 offset = searchPoint - linePoint;
        float theDot = Vector3.Dot(offset, unitVector);
        return linePoint + unitVector * theDot;
    }

    public void PlaceDebugCylinder(Vector3 positionTarget, Vector3 rayPos)
    {
        TargetBall.position = positionTarget;
        Vector3 cylinderPos = (positionTarget + rayPos) / 2;
        cylinderPivot.position = cylinderPos;

        cylinderPivot.LookAt(rayPos);
        float dist = (positionTarget - rayPos).magnitude;
        dist /= 2;
        cylinderPivot.localScale = new Vector3(1, 1, dist);
    }
}
