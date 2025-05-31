using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.Gaps.Gaps.Editor
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    public partial struct GapZEditorSystem : ISystem
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
            var zBuildingEditorJobEntity = new GapZEditorJobEntity()
            {
                Drawing = builder
            };
            zBuildingEditorJobEntity.ScheduleParallel();
            builder.DisposeAfter(state.Dependency);
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}