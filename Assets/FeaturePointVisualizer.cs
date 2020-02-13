using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FeaturePointVisualizer : MonoBehaviour
{
    [SerializeField]
    private Mesh particleMesh;
    [SerializeField]
    private Material particleMat;
    [SerializeField]
    private float particleScale;
    [SerializeField]
    public bool doPulse;
    [SerializeField]
    private float pulseSpeed;
    [SerializeField]
    private float pulseWaveSize;
    private float pulseProgression;

    void Update()
    {
        if(doPulse)
        {
            doPulse = false;
            particleMat.SetVector("_PulseCenter", AnnotationCursorBehavior.Instance.CenterDot.position);
            pulseProgression = 0;
        }
        pulseProgression += Time.deltaTime * pulseSpeed;
        particleMat.SetFloat("_PulseProgression", pulseProgression);
        particleMat.SetFloat("_PulseWaveSize", pulseWaveSize);
        Graphics.DrawMeshInstanced(particleMesh, 0, particleMat, GetFeaturePointMatrices().ToArray());
    }

    private IEnumerable<Matrix4x4> GetFeaturePointMatrices()
    {
        Quaternion lookRot = Quaternion.LookRotation(Camera.main.transform.forward);
        for (int i = 0; i < Frame.PointCloud.PointCount; i++)
        {
            yield return GetMatrix(Frame.PointCloud.GetPointAsStruct(i), lookRot);
        }
    }

    private Matrix4x4 GetMatrix(Vector3 point, Quaternion rotation)
    {
        return Matrix4x4.TRS(point, rotation, new Vector3(particleScale, particleScale, particleScale));
    }
}
