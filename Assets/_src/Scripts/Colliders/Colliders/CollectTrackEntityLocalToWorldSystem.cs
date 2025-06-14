using _src.Scripts.Colliders.Colliders.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace _src.Scripts.Colliders.Colliders
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct CollectTrackEntityLocalToWorldSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
#if UNITY_EDITOR
            state.RequireForUpdate<TrackCollidableEntityBuffer>();
#endif
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var collisionTrackBuffers = SystemAPI.GetSingletonBuffer<TrackCollidableEntityBuffer>();
            collisionTrackBuffers.Clear();
            foreach (var entity in SystemAPI.QueryBuilder()
                         .WithPresent<TrackCollidableTag>().Build()
                         .ToEntityArray(Allocator.Temp)
                    )
            {
                collisionTrackBuffers.Add(new()
                {
                    Entity = entity
                });
            }
        }
    }
}