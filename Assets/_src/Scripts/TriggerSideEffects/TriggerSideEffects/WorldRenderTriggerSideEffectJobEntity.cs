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
    [WithPresent(typeof(DestroyEntity))]
    [BurstCompile]
    public partial struct WorldRenderTriggerSideEffectJobEntity : IJobEntity
    {
        [ReadOnly] public NativeArray<TrackCollidableEntityBuffer>.ReadOnly CollisionTrackBuffer;
        [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorldLookup;
        [ReadOnly] public ComponentLookup<CollidePointOffsetComponent> PointContactOffsetLookup;
        
        [WriteOnly] public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly] public ObjectDefinitionRegistry ObjectDefinitionRegistry;

        private void Execute(
            [EntityIndexInQuery] int entityInQueryIndex, in Entity entity,
            in WorldRenderBounds worldRender,
            in TriggerSideEffectComponent triggerSideEffect,
            EnabledRefRW<DestroyEntity> destroyEntity
        )
        {
            foreach (var collisionTrackBuffer in CollisionTrackBuffer)
            {
                var target = collisionTrackBuffer.Entity;
                var position = LocalToWorldLookup[target].Position;
                var pointColliderComponent = PointContactOffsetLookup[target];

                var forwardOffset = new float3(0, pointColliderComponent.Center, pointColliderComponent.ForwardPre);
                bool isForwardTrigger = IsTrigger(
                    triggerSideEffect, TriggerType.SendForward,
                    worldRender, position, forwardOffset
                );
                if (isForwardTrigger)
                {
                    // PreCollisionQueue.Enqueue((entity, triggerSideEffect.PreSideEffect));
                }

                var isLegTouching = IsTrigger(
                    triggerSideEffect, TriggerType.HasInside,
                    worldRender, position, new float3(0, 0, 0)
                );
                
                bool isBodyIn = IsTrigger(
                    triggerSideEffect, TriggerType.EnableTop,
                    worldRender, position, new float3(0, pointColliderComponent.Center, 0)
                );

                if (isBodyIn)
                {
                    if (!triggerSideEffect.HasFlagFast(TriggerType.HasTop))
                    {
                        if (triggerSideEffect.HasFlagFast(TriggerType.DestroySelf)) destroyEntity.ValueRW = true;
                        Spawn(entityInQueryIndex, entity, entity, target, triggerSideEffect.OnInside);
                        return;
                    }
                }

                if (isLegTouching && triggerSideEffect.HasFlagFast(TriggerType.HasInside))
                {
                    if (triggerSideEffect.HasFlagFast(TriggerType.DestroySelf)) destroyEntity.ValueRW = true;
                    Spawn(entityInQueryIndex, entity, entity, target, triggerSideEffect.OnTop);
                }
            }
        }

        [BurstCompile]
        private bool IsTrigger(
            in TriggerSideEffectComponent triggerSideEffect, TriggerType triggerType,
            in WorldRenderBounds worldRender, float3 position, float3 offset
        )
        {
            if (!triggerSideEffect.HasFlagFast(triggerType)) return false;
            return worldRender.Value.Contains(position + offset);
        }

        [BurstCompile]
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