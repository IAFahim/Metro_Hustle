using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace _src.Scripts.CurveWorlds.CurveWorlds.Data
{
    [ExecuteAlways]
    public class CurveManager : MonoBehaviour
    {
        private static readonly int CurveVector = Shader.PropertyToID("_CurveVector");

        [Range(-.0015f, .0015f)] [SerializeField] private float x;
        [Range(-.0015f, .0015f)] [SerializeField] private float y;
        [Range(-.0015f, .0015f)] [SerializeField] private float z;
        [Range(99, 300)] [SerializeField] float range = 99;

        public static void UpdateBendingAmount(Vector3 curve) => Shader.SetGlobalVector(CurveVector, curve);

        private void OnValidate()
        {
            UpdateBendingAmount(new Vector3(x,z,y));
        }

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
            UpdateBendingAmount(new Vector3(x,z,y));
        }


        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
        }


        private void OnBeginCameraRendering(ScriptableRenderContext ctx, Camera cam)
        {
            if (!Application.isPlaying) return;
            cam.cullingMatrix = Matrix4x4.Ortho(-range, range, -range, range, 0.001f, range) *
                                cam.worldToCameraMatrix;
        }

        private static void OnEndCameraRendering(ScriptableRenderContext ctx, Camera cam) => cam.ResetCullingMatrix();
    }
}