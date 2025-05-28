using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.ZBuildings.ZBuildings.Editor
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor)]
    public partial struct ZBuildingEditorSystem : ISystem
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
            var zBuildingEditorJobEntity = new ZBuildingEditorJobEntity()
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