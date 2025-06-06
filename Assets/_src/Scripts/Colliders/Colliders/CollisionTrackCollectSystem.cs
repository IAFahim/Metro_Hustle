using _src.Scripts.Colliders.Colliders.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace _src.Scripts.Colliders.Colliders
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct CollisionTrackCollectSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CollisionTrackBuffer>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var collisionTrackBuffers = SystemAPI.GetSingletonBuffer<CollisionTrackBuffer>();
            var entities = SystemAPI.QueryBuilder().WithPresent<CollisionTrackComponent>().Build()
                .ToEntityArray(Allocator.Temp);
            collisionTrackBuffers.Clear();
            foreach (var entity in entities)
            {
                collisionTrackBuffers.Add(new()
                {
                    Entity = entity
                });
            }

            entities.Dispose();
        }
    }
}