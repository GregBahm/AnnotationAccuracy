using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FeaturePointVisualizer : MonoBehaviour
{
    public Mesh ParticleMesh;
    public Material ParticleMat;
    public float ParticleScale;

    void Update()
    {
        Graphics.DrawMeshInstanced(ParticleMesh, 0, ParticleMat, GetFeaturePointMatrices().ToArray());
    }

    private IEnumerable<Matrix4x4> GetFeaturePointMatrices()
    {
        for (int i = 0; i < Frame.PointCloud.PointCount; i++)
        {
            yield return GetMatrix(Frame.PointCloud.GetPointAsStruct(i));
        }
    }

    private Matrix4x4 GetMatrix(Vector3 point)
    {
        return Matrix4x4.TRS(point, Quaternion.identity, new Vector3(ParticleScale, ParticleScale, ParticleScale));
    }
}
