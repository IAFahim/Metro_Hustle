using _src.Scripts.Colliders.Colliders.Data;
using _src.Scripts.TriggerSideEffects.TriggerSideEffects.Data.enums;
using BovineLabs.Core.ObjectManagement;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects
{
    public partial struct WorldRenderTriggerSideEffectSystem : ISystem
    {
        public NativeQueue<(Entity entity, ESideEffect sideEffect)> PreCollisionQueue;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CollisionTrackBuffer>();
            PreCollisionQueue = new NativeQueue<(Entity entity, ESideEffect sideEffect)>(Allocator.Persistent);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PreCollisionQueue.Clear();
            SystemAPI.GetSingletonBuffer<CollisionTrackBuffer>();
            var worldRenderTriggerSideEffectJobEntity = new WorldRenderTriggerSideEffectJobEntity()
            {
                CollisionTrackBuffer =
                    SystemAPI.GetSingletonBuffer<CollisionTrackBuffer>().AsNativeArray().AsReadOnly(),
                LocalToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true),
                PointColliderLookup = SystemAPI.GetComponentLookup<PointColliderComponent>(true),
                PreCollisionQueue = PreCollisionQueue.AsParallelWriter(),
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