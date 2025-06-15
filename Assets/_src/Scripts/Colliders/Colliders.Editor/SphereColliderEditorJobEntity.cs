#if ALINE
using _src.Scripts.Colliders.Colliders.Data;
using _src.Scripts.Conditions.Conditions.Data;
using _src.Scripts.Positioning.Positioning.Data;
using _src.Scripts.Speeds.Speeds.Data;
using BovineLabs.Core.LifeCycle;
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
    [WithPresent(typeof(DestroyEntity))]
    public partial struct SphereColliderEditorJobEntity : IJobEntity
    {
        [ReadOnly] public NativeArray<TrackCollidableEntityBuffer>.ReadOnly TrackCollidableEntityBuffer;
        [ReadOnly] public ComponentLookup<LocalToWorld> LtwLookup;
        [WriteOnly] public CommandBuilder CommandBuilder;

        [BurstCompile]
        private void Execute(
            in LocalToWorld ltw, in SphereColliderComponent colliderComponent,
            ref LeftRightComponent leftRight, in ConditionSatisfiedFlagComponent conditionSatisfiedFlag,
            in SpeedTransferComponent speedTransferComponent,  EnabledRefRW<DestroyEntity> destroyFlag
        )
        {
            var inRange = 0;
            foreach (var entityBuffer in TrackCollidableEntityBuffer)
            {
                var targetPosition = LtwLookup[entityBuffer.Entity].Position;
                var difference = targetPosition - ltw.Position;
                var distance = math.lengthsq(difference);
                if (distance < colliderComponent.LengthSq && conditionSatisfiedFlag.Flag > 0)
                {
                    inRange++;
                    leftRight.Speed = speedTransferComponent.Speed;
                    if (distance < colliderComponent.DestroySq)
                    {
                        destroyFlag.ValueRW = true;
                    }
                    leftRight.Target = targetPosition.x;
                }
            }

            CommandBuilder.SphereOutline(
                ltw.Position, math.sqrt(colliderComponent.LengthSq),
                inRange == 0 ? Color.green : Color.yellow
            );
            CommandBuilder.SphereOutline(
                ltw.Position, math.sqrt(colliderComponent.DestroySq), Color.red
            );
        }
    }
}
#endif