using System;
using System.Collections.Generic;
using System.Linq;
using GoogleARCore;
using TriangleNet.Geometry;
using TriangleNet.Topology;
using UnityEngine;
using UnityEngine.UI;

public class MeshBuilder : MonoBehaviour
{
    public static MeshBuilder Instance { get; private set; }

    [SerializeField]
    private MeshFilter outputMeshFilter;

    private static Color[] triangleColors = new Color[] { Color.red, Color.green, Color.blue };

    private Mesh outputMesh;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        outputMeshFilter.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        outputMeshFilter.gameObject.SetActive(false);
    }

    private void Start()
    {
        outputMesh = new Mesh();
        outputMeshFilter.mesh = outputMesh;
    }

    public void BuildMeshFromFeaturePoints()
    {
        List<Vector3> rawPoints = GetPointCloudPositions().ToList();
        List<Vector3> psuedoHightmapPoints = rawPoints.Select(item => GetPointAsPsuedoHeightmap(item)).ToList();
        if (psuedoHightmapPoints.Count >= 3)
        {
            //psuedoHightmapPoints.AddRange(GetCornerPoints(psuedoHightmapPoints, rawPoints));
            TriangleNet.Mesh mesh = GetTriangulatedMesh(psuedoHightmapPoints);
            UpdateOutputMesh(mesh);
        }
    }

    private IEnumerable<Vector3> GetCornerPoints(List<Vector3> psuedoHightmapPoints, List<Vector3> rawPoints)
    {
        CornerSorter sorter = new CornerSorter();
        for (int i = 0; i < psuedoHightmapPoints.Count; i++)
        {
            Vector3 rawPoint = rawPoints[i];
            float height = psuedoHightmapPoints[i].y;
            sorter.SortPoint(rawPoint, height);
        }
        yield return sorter.GetUpperRightCornerPoint();
        yield return sorter.GetLowerRightCornerPoint();
        yield return sorter.GetUpperLeftCornerPoint();
        yield return sorter.GetLowerLeftCornerPoint();
    }

    private class CornerSorter
    {
        private float upperRight = Mathf.Infinity;
        public float UpperRightHeight { get; private set; }

        private float upperLeft = Mathf.Infinity;
        public float UpperLeftHeight { get; private set; }

        private float lowerRight = Mathf.Infinity;
        public float LowerRightHeight { get; private set; }

        private float lowerLeft = Mathf.Infinity;
        public float LowerLeftHeight { get; private set; }

        private static readonly Vector2 upperRightPoint = new Vector2(1, 1);
        private static readonly Vector2 upperLeftPoint = new Vector2(-1, 1);
        private static readonly Vector2 lowerRightPoint = new Vector2(1, -1);
        private static readonly Vector2 lowerLeftPoint = new Vector2(-1, -1);

        public void SortPoint(Vector3 point, float height)
        {
            Vector2 relativeScreenspace = GetPointInRelativeScreenspace(point);
            float upperRightDist = (relativeScreenspace - upperRightPoint).sqrMagnitude;
            float upperLeftDist = (relativeScreenspace - upperLeftPoint).sqrMagnitude;
            float lowerRightDist = (relativeScreenspace - lowerRightPoint).sqrMagnitude;
            float lowerLeftDist = (relativeScreenspace - lowerLeftPoint).sqrMagnitude;

            if (upperRight < upperRightDist)
            {
                upperRight = upperRightDist;
                UpperRightHeight = height;
            }
            if (upperLeft < upperLeftDist)
            {
                upperLeft = upperLeftDist;
                UpperLeftHeight = height;
            }
            if (lowerRight < lowerRightDist)
            {
                lowerRight = lowerRightDist;
                LowerRightHeight = height;
            }
            if (lowerLeft < lowerLeftDist)
            {
                lowerLeft = lowerLeftDist;
                LowerLeftHeight = height;
            }
        }

        public Vector3 GetUpperRightCornerPoint()
        {
            return GetCornerOutput(1, 1, UpperRightHeight);
        }

        public Vector3 GetLowerRightCornerPoint()
        {
            return GetCornerOutput(1, -1, LowerRightHeight);
        }

        public Vector3 GetUpperLeftCornerPoint()
        {
            return GetCornerOutput(-1, 1, UpperLeftHeight);
        }

        public Vector3 GetLowerLeftCornerPoint()
        {
            return GetCornerOutput(-1, -1, LowerLeftHeight);
        }

        private Vector3 GetCornerOutput(float xCorner, float yCorner, float height)
        {
            Vector3 screenPos = new Vector3((Camera.main.pixelWidth / 2) * xCorner, (Camera.main.pixelHeight / 2) * yCorner, 0);
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPos);
            Vector3 relativePoint = Camera.main.transform.worldToLocalMatrix * new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, 1);
            Vector3 ret = new Vector3(relativePoint.x, relativePoint.y, height);
            return SwizzleVertex(ret);
        }

        private Vector2 GetPointInRelativeScreenspace(Vector3 worldPos)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            float xParam = screenPos.x - Camera.main.pixelWidth / 2;
            float yParam = screenPos.y - Camera.main.pixelHeight / 2;
            return new Vector2(xParam, yParam);
        }
    }

    private IEnumerable<Vector3> GetPointCloudPositions()
    {
        for (int i = 0; i < Frame.PointCloud.PointCount; i++)
        {
            yield return Frame.PointCloud.GetPointAsStruct(i).Position;
        }
    }

    // The ConformingDelaunay conforms in the y dimension, but we want it to conform in the camera's z dimension
    private Vector3 GetPointAsPsuedoHeightmap(Vector3 worldPosition)
    {
        Vector3 cameraSpace = Camera.main.transform.worldToLocalMatrix * new Vector4(worldPosition.x, worldPosition.y, worldPosition.z, 1);
        return SwizzleVertex(cameraSpace);
    }

    public TriangleNet.Mesh GetTriangulatedMesh(List<Vector3> points)
    {
        Polygon polygon = new Polygon();

        foreach (var point in points)
        {
            polygon.Add(new Vertex(point));
        }

        TriangleNet.Meshing.ConstraintOptions options = new TriangleNet.Meshing.ConstraintOptions() { ConformingDelaunay = true };
        return (TriangleNet.Mesh)polygon.Triangulate(options);
    }

    private static Vector3 SwizzleVertex(Vector3 baseVert)
    {
        return new Vector3(baseVert.x, baseVert.z, baseVert.y);
    }

    public void UpdateOutputMesh(TriangleNet.Mesh mesh)
    {
        int indiciesCount = mesh.Triangles.Count * 3;

        Vector3[] vertices = new Vector3[indiciesCount];
        Color[] colors = new Color[indiciesCount];
        Vector3[] normals = new Vector3[indiciesCount];
        int[] triangles = new int[indiciesCount];

        IEnumerator<Triangle> triangleEnumerator = mesh.Triangles.GetEnumerator();
        for (int i = 0; i < indiciesCount; i += 3)
        {
            triangleEnumerator.MoveNext();
            Triangle triangle = triangleEnumerator.Current;
            Vector3 v0 = SwizzleVertex(triangle.GetVertex(2).original);
            Vector3 v1 = SwizzleVertex(triangle.GetVertex(1).original);
            Vector3 v2 = SwizzleVertex(triangle.GetVertex(0).original);

            vertices[i] = v0;
            vertices[i + 1] = v1;
            vertices[i + 2] = v2;

            triangles[i] = i;
            triangles[i + 1] = i + 1;
            triangles[i + 2] = i + 2;

            Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0);
            normals[i] = normal;
            normals[i + 1] = normal;
            normals[i + 2] = normal;

            colors[i] = triangleColors[0];
            colors[i + 1] = triangleColors[1];
            colors[i + 2] = triangleColors[2];
        }

        outputMesh.Clear();
        outputMesh.vertices = vertices;
        outputMesh.triangles = triangles;
        outputMesh.normals = normals;
        outputMesh.colors = colors;
    }
}
