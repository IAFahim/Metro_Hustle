#if ALINE
using Drawing;
#endif
using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.SplineColliders.SplineColliders.Editor
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    public partial struct SplineColliderEditorSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
#if ALINE
            CommandBuilder builder = DrawingManager.GetBuilder();
            var splineColliderEditorJobEntity = new SplineColliderEditorJobEntity()
            {
                Drawing = builder
            };
            splineColliderEditorJobEntity.ScheduleParallel();
            builder.DisposeAfter(state.Dependency);
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}