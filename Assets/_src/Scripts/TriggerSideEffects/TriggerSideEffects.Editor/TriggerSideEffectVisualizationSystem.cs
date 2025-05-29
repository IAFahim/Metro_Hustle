using _src.Scripts.Colliders.Colliders.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects.Editor
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    public partial struct TriggerSideEffectVisualizationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
#if ALINE
            quaternion editorCamRot = quaternion.identity;
            if (UnityEditor.SceneView.lastActiveSceneView != null)
            {
                editorCamRot = UnityEditor.SceneView.lastActiveSceneView.camera.transform.rotation;
            }
            var builder = Drawing.DrawingManager.GetBuilder();
            var triggerSideEffectVisualizationJobEntity = new TriggerSideEffectVisualizationJobEntity()
            {
                Drawing = builder,
                EditorCameraRotation = editorCamRot,
            };
            triggerSideEffectVisualizationJobEntity.ScheduleParallel();
            builder.DisposeAfter(state.Dependency);
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}