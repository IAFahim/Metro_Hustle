using UnityEngine;
using UnityEngine.Rendering;

namespace _src.Scripts.CurveWorlds.CurveWorlds.Data
{
    [ExecuteAlways]
    public class CurveManager : MonoBehaviour
    {
        private static readonly int CurveVector = Shader.PropertyToID("_CurveVector");


        [SerializeField] private Vector3 curveVector;

        public static void UpdateBendingAmount(Vector3 curve) => Shader.SetGlobalVector(CurveVector, curve);

        private void OnValidate()
        {
            UpdateBendingAmount(curveVector);
        }


        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
            UpdateBendingAmount(curveVector);
        }


        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
        }


        private static void OnBeginCameraRendering(ScriptableRenderContext ctx, Camera cam)
        {
            if (!Application.isPlaying) return;
            cam.cullingMatrix = Matrix4x4.Ortho(-99, 99, -99, 99, 0.001f, 99) *
                                cam.worldToCameraMatrix;
        }

        private static void OnEndCameraRendering(ScriptableRenderContext ctx, Camera cam) => cam.ResetCullingMatrix();
    }
}