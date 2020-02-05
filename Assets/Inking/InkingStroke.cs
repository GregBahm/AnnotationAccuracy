using System.Collections.Generic;
using UnityEngine;

public class InkingStroke : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;
    
    private const int pointsPerSegment = 5;
    private Queue<Vector3> controlPoints = new Queue<Vector3>();
    private Vector3 lastPoint;
    private float startWidth;
    private int totalPointCount = 0;

    public void AddPoint(Vector3 point, bool isLastPoint)
    {
        // Add initial point
        if (totalPointCount == 0)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(0, point);
            totalPointCount++;
        }

        // Add the new point to the control points queue
        controlPoints.Enqueue(point);

        // If there are three control points already stored, then draw a segment
        if (controlPoints.Count == 3)
        {
            Vector3[] controlPointsArr = controlPoints.ToArray();

            Vector3 p0, p1, p2;
            // determine control points of segment
            p0 = 0.5f * (controlPointsArr[0] + controlPointsArr[1]);
            p1 = controlPointsArr[1];
            p2 = 0.5f * (controlPointsArr[1] + controlPointsArr[2]);

            // set points of quadratic Bezier curve
            Vector3 position;
            float t;
            float pointStep = 1.0f / pointsPerSegment;

            if (isLastPoint)
            {
                pointStep = 1.0f / (pointsPerSegment - 1.0f);
            }

            for (int i = 0; i < pointsPerSegment; i++)
            {
                t = i * pointStep;
                position = (1.0f - t) * (1.0f - t) * p0
                + 2.0f * (1.0f - t) * t * p1 + t * t * p2;

                AddPointToLineRenderer(position);
            }

            controlPoints.Dequeue();
        }
        else if (isLastPoint && controlPoints.Count < 3)
        {
            AddPointToLineRenderer(point);
        }
        else if (controlPoints.Count > 3)
        {
            Debug.LogError("Error: Bezier_Spline: There should never be more control points in the the queue than 3", this);
        }
    }

    private void AddPointToLineRenderer(Vector3 point)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(totalPointCount, point);
        totalPointCount++;
    }
}