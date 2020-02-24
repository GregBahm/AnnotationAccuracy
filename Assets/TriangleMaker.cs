using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriangleMaker : MonoBehaviour
{
    [SerializeField]
    private MeshFilter meshFilter;

    public static TriangleMaker Instance { get; private set; }

    private Mesh triangleMesh;
    public Plane TrianglePlane { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        triangleMesh = new Mesh();
        triangleMesh.vertices = new Vector3[3];
        triangleMesh.triangles = new int[] { 0, 1, 2 };
        triangleMesh.colors = new Color[] { Color.red, Color.green, Color.blue };
        meshFilter.mesh = triangleMesh;

    }

    public void DoUpdate()
    {
        Vector2 cursorPosition = GetCursorPixelPosition();
        Datum[] data = GetData(cursorPosition).ToArray();
        if (data.Length >= 3)
        {
            PointSorter sortedPoints = new PointSorter(data);
            if(sortedPoints.TriangleFound)
            {
                UpdateMesh(sortedPoints);
            }
        }
    }

    private Vector2 GetCursorPixelPosition()
    {
        float xPos = Camera.main.pixelWidth / 2;
        float yPos = Camera.main.pixelHeight / 2;
        return new Vector2(xPos, yPos);
    }

    private void UpdateMesh(PointSorter sortedPoints)
    {
        TrianglePlane = new Plane(sortedPoints.FirstPoint.WorldPos, sortedPoints.SecondPoint.WorldPos, sortedPoints.ThirdPoint.WorldPos);
        triangleMesh.vertices = new Vector3[] { sortedPoints.FirstPoint.WorldPos, sortedPoints.SecondPoint.WorldPos, sortedPoints.ThirdPoint.WorldPos };
        triangleMesh.normals = new Vector3[] { TrianglePlane.normal, TrianglePlane.normal, TrianglePlane.normal };
        triangleMesh.RecalculateBounds();
    }

    private IEnumerable<Datum> GetData(Vector2 cursorPixelPosition)
    {
        for (int i = 0; i < Frame.PointCloud.PointCount; i++)
        {
            yield return GetDatum(Frame.PointCloud.GetPointAsStruct(i).Position, i, cursorPixelPosition);
        }
    }

    private Datum GetDatum(Vector3 pointPosition, int index, Vector2 cursorPixelPosition)
    {
        Vector3 screenPointInPixels = Camera.main.WorldToScreenPoint(pointPosition);
        float xParam = screenPointInPixels.x - cursorPixelPosition.x;
        float yParam = screenPointInPixels.y - cursorPixelPosition.y;
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
        public Datum SecondPoint { get; private set; }
        public Datum[] ThirdPointCandidates { get; }
        public Datum ThirdPoint { get; private set; }
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
                    ThirdPoint = GetClosestPointToCenter(ThirdPointCandidates);
                    SortPointsClockwise();
                    TriangleFound = true;
                }
            }
        }

        private void SortPointsClockwise()
        {
            float angleA = Vector2.SignedAngle(FirstPoint.ToCursorNormalized, SecondPoint.ToCursorNormalized);
            float angleB = Vector2.SignedAngle(FirstPoint.ToCursorNormalized, ThirdPoint.ToCursorNormalized);
            if(angleA < angleB)
            {
                Datum thirdPoint = ThirdPoint;
                ThirdPoint = SecondPoint;
                SecondPoint = thirdPoint;
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
