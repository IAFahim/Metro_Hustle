using _src.Scripts.Colliders.Colliders.Data;
using _src.Scripts.TriggerSideEffects.TriggerSideEffects.Data;
using BovineLabs.Core.LifeCycle;
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

                bool isForwardTrigger = IsTrigger(
                    triggerSideEffectSpawn, TriggerType.HasForwardAndEnable,
                    worldRender, targetLtw.Position, forwardOffset
                );
                if (isForwardTrigger)
                {
                    Spawn(entityInQueryIndex, entity, entity, target, triggerSideEffectSpawn.OnForwardPre);
                    return;
                }

                var isInsideTrigger = IsTrigger(
                    triggerSideEffectSpawn, TriggerType.HasInsideAndEnable,
                    worldRender, targetLtw.Position, new float3(0, 0, 0)
                );
                if (!isInsideTrigger) return;

                var upOffset = new float3(0, pointColliderComponent.UpOffset, 0);
                bool isLegInTrigger = IsTrigger(
                    triggerSideEffectSpawn, TriggerType.HasTopAndEnable,
                    worldRender, targetLtw.Position, upOffset
                );
                if (isLegInTrigger)
                {
                    Spawn(entityInQueryIndex, entity, entity, target, triggerSideEffectSpawn.OnTop);
                    return;
                }

                Spawn(entityInQueryIndex, entity, entity, target, triggerSideEffectSpawn.OnInside);
            }
        }

        [BurstCompile]
        private bool IsTrigger(
            in TriggerSideEffectSpawnComponent triggerSideEffectSpawn, TriggerType triggerType,
            in WorldRenderBounds worldRender, float3 position, float3 offset
        )
        {
            if ((triggerSideEffectSpawn.TriggerType & triggerType) != triggerType) return false;
            return worldRender.Value.Contains(position + offset);
        }

        private bool Spawn(int entityInQueryIndex, Entity owner, Entity source, Entity target, ObjectId objectId)
        {
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