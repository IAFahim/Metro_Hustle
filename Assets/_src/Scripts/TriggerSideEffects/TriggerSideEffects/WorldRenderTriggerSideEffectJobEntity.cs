using _src.Scripts.Colliders.Colliders.Data;
using _src.Scripts.TriggerSideEffects.TriggerSideEffects.Data;
using BovineLabs.Core.ObjectManagement;
using BovineLabs.Reaction.Data.Core;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects
{
    [BurstCompile]
    public partial struct WorldRenderTriggerSideEffectJobEntity : IJobEntity
    {
        [ReadOnly] public NativeArray<CollisionTrackBuffer>.ReadOnly CollisionTrackBuffer;
        [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorldLookup;
        [ReadOnly] public ComponentLookup<PointColliderComponent> PointColliderLookup;
        [WriteOnly] public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly] public ObjectDefinitionRegistry ObjectDefinitionRegistry;

        private void Execute(
            [EntityIndexInQuery] int entityInQueryIndex, in Entity entity,
            in WorldRenderBounds worldRender,
            in TriggerSideEffectSpawnComponent triggerSideEffectSpawn
        )
        {
            foreach (var collisionTrackBuffer in CollisionTrackBuffer)
            {
                var target = collisionTrackBuffer.Entity;
                var targetLtw = LocalToWorldLookup[target];
                var pointColliderComponent = PointColliderLookup[target];
                var forwardOffset = new float3(0, pointColliderComponent.ForwardPre, 0);

                if (Trigger(entityInQueryIndex, TriggerType.HasForwardAndEnable, 
                    triggerSideEffectSpawn, worldRender, targetLtw.Position, forwardOffset, 
                    entity, entity, target, triggerSideEffectSpawn.OnForwardPre))
                {
                    return;
                }

                if (!worldRender.Value.Contains(targetLtw.Position)) return;

                var upOffset = new float3(0, pointColliderComponent.UpOffset, 0);
                bool legIn = worldRender.Value.Contains(targetLtw.Position + upOffset);

                ObjectId objectToSpawn = legIn ? triggerSideEffectSpawn.OnInside : triggerSideEffectSpawn.OnTop;

                var entityPrefab = ObjectDefinitionRegistry[objectToSpawn];
                var instantiate = ECB.Instantiate(entityInQueryIndex, entityPrefab);
                ECB.SetComponent(entityInQueryIndex, instantiate, new Targets()
                {
                    Owner = entity,
                    Source = entity,
                    Target = target,
                });
            }
        }

        [BurstCompile]
        private bool Trigger(
            int entityInQueryIndex,
            TriggerType triggerType,
            in TriggerSideEffectSpawnComponent triggerSideEffectSpawn, 
            in WorldRenderBounds worldRender, 
            float3 position, 
            float3 offset, 
            in Entity owner, 
            in Entity source, 
            in Entity target,
            ObjectId objectId
        )
        {
            if ((triggerSideEffectSpawn.TriggerType & triggerType) != triggerType) return false;
            
            var offsetPosition = position + offset;
            if (!worldRender.Value.Contains(offsetPosition)) return false;
            
            var entityPrefab = ObjectDefinitionRegistry[objectId];
            var instantiate = ECB.Instantiate(entityInQueryIndex, entityPrefab);
            ECB.SetComponent(entityInQueryIndex, instantiate, new Targets()
            {
                Owner = owner,
                Source = source,
                Target = target,
            });
            return true;
        }
    }
}