using _src.Scripts.Colliders.Colliders.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace _src.Scripts.Colliders.Colliders.Editor
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor|WorldSystemFilterFlags.Default)]
    public partial struct SphereColliderEditorSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TrackCollidableEntityBuffer>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
#if ALINE
            // var builder = Drawing.DrawingManager.GetBuilder();
            var sphereColliderEditorJobEntity = new SphereColliderEditorJobEntity
            {
                TrackCollidableEntityBuffer = SystemAPI.GetSingletonBuffer<TrackCollidableEntityBuffer>().ToNativeArray(Allocator.Temp).AsReadOnly()
            };
            sphereColliderEditorJobEntity.ScheduleParallel(state.Dependency);
            // builder.DisposeAfter(state.Dependency);
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}