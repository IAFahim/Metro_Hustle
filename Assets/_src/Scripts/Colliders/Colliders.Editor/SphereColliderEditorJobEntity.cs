#if ALINE
using _src.Scripts.Conditions.Conditions.Data;
using BovineLabs.Core.LifeCycle;
using BovineLabs.Reaction.Data.Conditions;
using BovineLabs.Reaction.Data.Core;
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
    [WithPresent(typeof(DestroyEntity), typeof(ConditionAllActive), typeof(ConditionSatisfied))]
    public partial struct SphereColliderEditorJobEntity : IJobEntity
    {
        [WriteOnly] public CommandBuilder CommandBuilder;
        [ReadOnly] public NativeArray<(Entity targetEntity, float3 position, float range)>.ReadOnly TargetInfos;

        [BurstCompile]
        private void Execute(
            in LocalToWorld ltw,
            EnabledRefRO<ConditionAllActive> conditionAllActive,
            ref Targets targets,
            EnabledRefRW<ConditionSatisfied> conditionSatisfied
        )
        {
            if (!conditionAllActive.ValueRO) return;
            foreach (var (targetEntity, position, range) in TargetInfos)
            {
                var difference = position - ltw.Position;
                var distance = math.lengthsq(difference);
                bool inRange = distance < range;
                if (inRange)
                {
                    if (!conditionSatisfied.ValueRO) conditionSatisfied.ValueRW = true;
                    targets.Target = targetEntity;
                }

                CommandBuilder.SphereOutline(
                    ltw.Position, math.sqrt(range),
                    inRange ? Color.red : Color.green
                );
            }
        }
    }
}
#endif