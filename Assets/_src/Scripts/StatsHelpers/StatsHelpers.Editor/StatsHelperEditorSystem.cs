using BovineLabs.Stats.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.StatsHelpers.StatsHelpers.Editor
{
    public partial struct StatsHelperEditorSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
#if ALINE
            var builder = Drawing.DrawingManager.GetBuilder();
            quaternion editorCamRot = quaternion.identity;
            if (UnityEditor.SceneView.lastActiveSceneView != null)
            {
                editorCamRot = UnityEditor.SceneView.lastActiveSceneView.camera.transform.rotation;
            }

            var statsHelperEditorJobEntity = new StatsHelperEditorJobEntity()
            {
                Drawing = builder,
                StatsBuffer = SystemAPI.GetBufferLookup<Stat>()
                    
            };
            statsHelperEditorJobEntity.ScheduleParallel();
            builder.DisposeAfter(state.Dependency);
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}