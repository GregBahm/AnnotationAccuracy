using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TriangleMaker : MonoBehaviour
{
    private Mesh triangleMesh;
    private readonly Vector3[] meshPoints = new Vector3[3] { Vector3.zero, Vector3.up, Vector3.left };
    public Plane TrianglePlane { get; private set; }
    
    private void Start()
    {
        triangleMesh = new Mesh();
        triangleMesh.vertices = meshPoints;
        triangleMesh.triangles = new int[] { 0, 1, 2 };
        GetComponent<MeshFilter>().mesh = triangleMesh;

    }

    private void Update()
    {
        Datum[] data = GetData().ToArray();
        if (data.Length >= 3)
        {
            PointSorter sortedPoints = new PointSorter(data);
            if(sortedPoints.TriangleFound)
            {
                UpdateMesh(sortedPoints);
            }
        }
    }

    private void UpdateMesh(PointSorter sortedPoints)
    {
        meshPoints[0] = sortedPoints.FirstPoint.WorldPos;
        meshPoints[1] = sortedPoints.SecondPoint.WorldPos;
        meshPoints[2] = sortedPoints.ThirdPont.WorldPos;
        triangleMesh.vertices = meshPoints;
        triangleMesh.RecalculateBounds();
        TrianglePlane = new Plane(sortedPoints.FirstPoint.WorldPos, sortedPoints.SecondPoint.WorldPos, sortedPoints.ThirdPont.WorldPos);
    }

    private IEnumerable<Datum> GetData()
    {
        for (int i = 0; i < Frame.PointCloud.PointCount; i++)
        {
            yield return GetDatum(Frame.PointCloud.GetPointAsStruct(i).Position, i);
        }
    }

    private Datum GetDatum(Vector3 pointPosition, int index)
    {
        Vector3 screenPointInPixels = Camera.main.WorldToScreenPoint(pointPosition);
        float xParam = screenPointInPixels.x - Camera.main.pixelWidth / 2;
        float yParam = screenPointInPixels.y - Camera.main.pixelHeight / 2;
        Vector2 fromScreenCenter = new Vector2(xParam, yParam);

        float cursorDist = fromScreenCenter.magnitude;
        return new Datum(index,
            pointPosition, 
            cursorDist, 
            fromScreenCenter.normalized);
    }

    private class PointSorter
    {
        public Datum FirstPoint { get; }
        public Datum[] SecondPointCandidates { get; }
        public Datum SecondPoint { get; }
        public Datum[] ThirdPointCandidates { get; }
        public Datum ThirdPont { get; }
        public bool TriangleFound { get; }

        public PointSorter(IEnumerable<Datum> sourceData)
        {
            FirstPoint = GetClosestPointToCenter(sourceData);
            SecondPointCandidates = GetSecondPointCandidates(sourceData, FirstPoint).ToArray();
            if(SecondPointCandidates.Length > 0)
            {
                SecondPoint = GetClosestPointToCenter(SecondPointCandidates);
                ThirdPointCandidates = GetThirdPointCandidates(FirstPoint, SecondPoint, sourceData).ToArray();
                if(ThirdPointCandidates.Any())
                {
                    ThirdPont = GetClosestPointToCenter(ThirdPointCandidates);
                    TriangleFound = true;
                }
            }
        }

        private static Datum GetClosestPointToCenter(IEnumerable<Datum> sourceData)
        {
            return sourceData.OrderBy(item => item.CursorDist).First();
        }
        
        public static IEnumerable<Datum> GetSecondPointCandidates(IEnumerable<Datum> sourceData, Datum firstPoint)
        {
            foreach (Datum item in sourceData.Where(item => item.PointCloudIndex != firstPoint.PointCloudIndex))
            {
                float theDot = Vector2.Dot(firstPoint.ToCursorNormalized, item.ToCursorNormalized);
                if(theDot < 0)
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<Datum> GetThirdPointCandidates(Datum firstPoint, Datum secondPoint, IEnumerable<Datum> sourceData)
        {
            foreach (Datum item in sourceData.Where(item => item.PointCloudIndex != firstPoint.PointCloudIndex
                                                            && item.PointCloudIndex != secondPoint.PointCloudIndex))
            {
                if(IsThirdPointCandidate(firstPoint.ToCursorNormalized, secondPoint.ToCursorNormalized, item.ToCursorNormalized))
                {
                    yield return item;
                }
            }
        }

        private static bool IsThirdPointCandidate(Vector2 pointA, Vector2 pointB, Vector2 pointC)
        {
            float angleAB = Vector2.SignedAngle(pointA, pointB);
            float angleAC = Vector2.SignedAngle(pointA, pointC);
            bool passesTheFirstSeal = (angleAB > 0) != (angleAC > 0);
            if(passesTheFirstSeal)
            {
                float angleBC = Vector2.SignedAngle(pointB, pointC);
                float angleBA = Vector2.SignedAngle(pointB, pointA);
                return (angleBC > 0) != (angleBA > 0);
            }
            return false;
        }
    }

    private struct Datum
    {
        public int PointCloudIndex { get; }
        public Vector3 WorldPos { get; }
        public float CursorDist { get; }
        public Vector2 ToCursorNormalized { get; }

        public Datum(int pointCloudIndex, 
            Vector3 worldPos, 
            float cursorDist, 
            Vector2 toCursorNormalized)
        {
            PointCloudIndex = pointCloudIndex;
            WorldPos = worldPos;
            CursorDist = cursorDist;
            ToCursorNormalized = toCursorNormalized;
        }
    }
}
