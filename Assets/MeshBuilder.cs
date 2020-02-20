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

    private void Start()
    {
        outputMesh = new Mesh();
        outputMeshFilter.mesh = outputMesh;
    }

    public void BuildMeshFromFeaturePoints()
    {
        List<Vector3> psuedoHightmapPoints = GetPointCloudPositions().ToList();
        //psuedoHightmapPoints.AddRange(GetCornerPoints(psuedoHightmapPoints));
        if (psuedoHightmapPoints.Count >= 3)
        {
            TriangleNet.Mesh mesh = GetTriangulatedMesh(psuedoHightmapPoints);
            UpdateOutputMesh(mesh);
        }
    }

    //private IEnumerable<Vector3> GetCornerPoints(List<Vector3> psuedoHightmapPoints)
    //{
    //    float minX;
    //    float minZ;
    //    float maxX;
    //    float maxZ;
    //}

    private IEnumerable<Vector3> GetPointCloudPositions()
    {
        for (int i = 0; i < Frame.PointCloud.PointCount; i++)
        {
            Vector3 worldPosition = Frame.PointCloud.GetPointAsStruct(i).Position;
            yield return GetPointAsPsuedoHeightmap(worldPosition);
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

    private Vector3 SwizzleVertex(Vector3 baseVert)
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
