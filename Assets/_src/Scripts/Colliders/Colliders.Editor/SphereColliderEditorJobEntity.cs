#if ALINE
using _src.Scripts.Colliders.Colliders.Data;
using _src.Scripts.Conditions.Conditions.Data;
using BovineLabs.Core.LifeCycle;
using BovineLabs.Reaction.Data.Conditions;
using BovineLabs.Reaction.Data.Core;
using BovineLabs.Stats.Data;
using Drawing;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Editor
{
    [BurstCompile]
    [WithPresent(typeof(DestroyEntity), typeof(ConditionAllActive), typeof(ConditionSatisfied))]
    public partial struct SphereColliderEditorJobEntity : IJobEntity
    {
        [ReadOnly] public NativeArray<TrackCollidableEntityBuffer>.ReadOnly TrackCollidableEntityBuffer;
        [ReadOnly] public ComponentLookup<LocalToWorld> LtwLookup;
        [WriteOnly] public CommandBuilder CommandBuilder;

        [BurstCompile]
        private void Execute(
            in LocalToWorld ltw,
            EnabledRefRO<ConditionAllActive> conditionAllActive,
            ref Targets targets,
            in DynamicBuffer<Stat> statBuffer,
            EnabledRefRW<ConditionSatisfied> conditionSatisfied
        )
        {
            if (!conditionAllActive.ValueRO) return;
            var inRange = 0;
            var stats = statBuffer.AsMap();
            stats.TryGetValue((byte)EStat.RangeSq, out var range);
            
            foreach (var entityBuffer in TrackCollidableEntityBuffer)
            {
                var targetPosition = LtwLookup[entityBuffer.Entity].Position;
                var difference = targetPosition - ltw.Position;
                var distance = math.lengthsq(difference);
                if (distance < range.Value)
                {
                    if (!conditionSatisfied.ValueRO) conditionSatisfied.ValueRW = true;
                    targets.Target = entityBuffer.Entity;
                    inRange++;
                }
            }

            CommandBuilder.SphereOutline(
                ltw.Position, math.sqrt(range.Value),
                inRange == 0 ? Color.green : Color.blue
            );
            // CommandBuilder.SphereOutline(
            //     ltw.Position, math.sqrt(colliderComponent.DestroySq), Color.red
            // );
        }
    }
}
#endif