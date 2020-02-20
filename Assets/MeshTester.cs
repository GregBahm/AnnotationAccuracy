using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshTester : MonoBehaviour
{
    public bool Regenerate = true;
    public int PointCount = 1000;

    void Start()
    {

    }

    private void Update()
    {
        if(Regenerate)
        {
            Regenerate = false;
            List<Vector3> points = CreateRandomPoints().ToList();
            var mesh = MeshBuilder.Instance.GetTriangulatedMesh(points);
            MeshBuilder.Instance.UpdateOutputMesh(mesh);
        }
    }

    private IEnumerable<Vector3> CreateRandomPoints()
    {
        for (int i = 0; i < PointCount; i++)
        {
            float randX = UnityEngine.Random.value;
            float randY = UnityEngine.Random.value;
            float randZ = UnityEngine.Random.value;
            yield return new Vector3(randX, randY, randZ);
        }
    }
}
