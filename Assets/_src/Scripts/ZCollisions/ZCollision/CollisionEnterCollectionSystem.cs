using _src.Scripts.ZCollisions.ZCollision.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace _src.Scripts.ZCollisions.ZCollision
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct CollisionEnterCollectionSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityQuery = SystemAPI.QueryBuilder().WithPresent<CollisionEnterComponent>().Build();
            var buffer = SystemAPI.GetSingletonBuffer<CollisionEnterEntityBuffer>();
            buffer.Clear();
            foreach (var entity in entityQuery.ToEntityArray(Allocator.Temp))
            {
                buffer.Add(new CollisionEnterEntityBuffer()
                {
                    Entity = entity
                });
            }
        }
    }
}