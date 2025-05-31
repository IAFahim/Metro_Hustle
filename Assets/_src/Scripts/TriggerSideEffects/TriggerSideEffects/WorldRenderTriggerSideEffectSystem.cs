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
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var worldRenderTriggerSideEffectJobEntity = new WorldRenderTriggerSideEffectJobEntity()
            {
                CollisionTrackBuffer =
                    SystemAPI.GetSingletonBuffer<CollisionTrackBuffer>().AsNativeArray().AsReadOnly(),
                LocalToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true),
                PointColliderLookup = SystemAPI.GetComponentLookup<PointColliderComponent>(true),
                ECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
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