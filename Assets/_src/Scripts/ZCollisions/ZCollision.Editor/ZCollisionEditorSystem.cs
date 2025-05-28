using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.ZCollisions.ZCollision.Editor
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    public partial struct ZCollisionEditorSystem : ISystem
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
            var zCollisionEditorJobEntity = new ZCollisionEditorJobEntity()
            {
                Drawing = builder
            };
            zCollisionEditorJobEntity.ScheduleParallel();
            builder.DisposeAfter(state.Dependency);
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}