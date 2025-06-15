#if ALINE
using _src.Scripts.Colliders.Colliders.Data;
using _src.Scripts.Positioning.Positioning.Data;
using _src.Scripts.Speeds.Speeds.Data;
using BovineLabs.Core.LifeCycle;
using BovineLabs.Reaction.Data.Conditions;
using BovineLabs.Reaction.Data.Core;
using BovineLabs.Stats.Data;
using Drawing;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Editor
{
    [BurstCompile]
    [WithPresent(typeof(DestroyEntity), typeof(ConditionAllActive))]
    public partial struct SphereColliderEditorJobEntity : IJobEntity
    {
        [ReadOnly] public NativeArray<TrackCollidableEntityBuffer>.ReadOnly TrackCollidableEntityBuffer;
        [ReadOnly] public ComponentLookup<LocalToWorld> LtwLookup;
        [WriteOnly] public CommandBuilder CommandBuilder;

        [BurstCompile]
        private void Execute(
            in LocalToWorld ltw,
            // in SphereColliderComponent colliderComponent,
            ref LeftRightComponent leftRight,
            EnabledRefRO<ConditionAllActive> conditionAllActive,
            in SpeedTransferComponent speedTransferComponent, 
            EnabledRefRW<DestroyEntity> destroyFlag,
            ref Targets targets,
            in DynamicBuffer<Stat> statBuffer) 
        {
            var inRange = 0;
            var stats = statBuffer.AsMap();
            stats.TryGetValue(6, out var range);
            foreach (var entityBuffer in TrackCollidableEntityBuffer)
            {
                var targetPosition = LtwLookup[entityBuffer.Entity].Position;
                var difference = targetPosition - ltw.Position;
                var distance = math.lengthsq(difference);
                if (distance < range.Value && conditionAllActive.ValueRO)
                {
                    inRange++;
                    leftRight.Speed = speedTransferComponent.Speed;
                    targets.Target = entityBuffer.Entity;
                    leftRight.Target = targetPosition.x;
                    
                    // if (distance < colliderComponent.DestroySq)
                    // {
                    //     destroyFlag.ValueRW = true;
                    // }
                }
            }

            CommandBuilder.SphereOutline(
                ltw.Position, math.sqrt(range.Value),
                inRange == 0 ? Color.green : Color.yellow
            );
            // CommandBuilder.SphereOutline(
            //     ltw.Position, math.sqrt(colliderComponent.DestroySq), Color.red
            // );
        }
    }
}
#endif