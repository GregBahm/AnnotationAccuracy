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
        List<Vector3> points = GetPointCloudPositions().ToList();

        if (points.Count >= 3)
        {
            TriangleNet.Mesh mesh = GetTriangulatedMesh(points);
            UpdateOutputMesh(mesh);
        }
    }

    private IEnumerable<Vector3> GetPointCloudPositions()
    {
        for (int i = 0; i < Frame.PointCloud.PointCount; i++)
        {
            yield return Frame.PointCloud.GetPointAsStruct(i).Position;
        }
    }

    private TriangleNet.Mesh GetTriangulatedMesh(List<Vector3> points)
    {
        Polygon polygon = new Polygon();

        foreach (var point in points)
        {
            polygon.Add(new Vertex(point));
        }

        TriangleNet.Meshing.ConstraintOptions options = new TriangleNet.Meshing.ConstraintOptions() { ConformingDelaunay = false };
        return (TriangleNet.Mesh)polygon.Triangulate(options);
    }

    private void UpdateOutputMesh(TriangleNet.Mesh mesh)
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
            Vertex v0 = triangle.GetVertex(2);
            Vertex v1 = triangle.GetVertex(1);
            Vertex v2 = triangle.GetVertex(0);

            vertices[i] = v0.original;
            vertices[i + 1] = v1.original;
            vertices[i + 2] = v2.original;

            triangles[i] = i;
            triangles[i + 1] = i + 1;
            triangles[i + 2] = i + 2;

            Vector3 normal = Vector3.Cross(v1.original - v0.original, v2.original - v0.original);
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
