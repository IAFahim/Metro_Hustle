using _src.Scripts.Colliders.Colliders.Data;
using BovineLabs.Core.ObjectManagement;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects
{
    public partial struct WorldRenderTriggerSideEffectSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TrackCollidableEntityBuffer>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var worldRenderTriggerSideEffectJobEntity = new WorldRenderTriggerSideEffectJobEntity()
            {
                CollisionTrackBuffer =
                    SystemAPI.GetSingletonBuffer<TrackCollidableEntityBuffer>().AsNativeArray().AsReadOnly(),
                ECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                LocalToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true),
                PointContactOffsetLookup = SystemAPI.GetComponentLookup<CollidePointOffsetComponent>(true),
                ObjectDefinitionRegistry = SystemAPI.GetSingleton<ObjectDefinitionRegistry>()
                 
            };
            worldRenderTriggerSideEffectJobEntity.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}