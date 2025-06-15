using _src.Scripts.Colliders.Colliders.Data;
using _src.Scripts.Conditions.Conditions.Data;
using _src.Scripts.Easings.Easings.Data;
using _src.Scripts.Positioning.Positioning.Data;
using BovineLabs.Reaction.Data.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.Positioning.Positioning
{
    [WithPresent(typeof(StartPositionRecordComponent), typeof(EaseComponent))]
    [WithAll(typeof(ConditionSatisfied))]
    public partial struct EasePositioningJobEntity : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentLookup<LocalToWorld> LtwLookup;
        [ReadOnly] public float DeltaTime;

        private void Execute(
            Entity entity,
            ref EaseComponent easeComponent,
            EnabledRefRW<StartPositionRecordComponent> startPositionRecorded,
            ref StartPositionRecordComponent startPosition,
            in Targets targets,
            EnabledRefRW<EaseComponent> ease,
            in CollidePointOffsetComponent collidePointOffset
        )
        {
            var ltw = LtwLookup.GetRefRW(entity);
            if (startPositionRecorded.ValueRO == false)
            {
                startPosition.Position = LtwLookup[entity].Position;
                startPositionRecorded.ValueRW = true;
            }

            var targetPosition = LtwLookup[targets.Target].Position;
            targetPosition.y += collidePointOffset.Center;
            if (easeComponent.Elapsed >= easeComponent.Duration)
            {
                ltw.ValueRW.Value.c3.xyz = targetPosition;
                ease.ValueRW = false;
                return;
            }

            var t = easeComponent.Elapsed / easeComponent.Duration;
            easeComponent.Elapsed += (half)DeltaTime;
            ltw.ValueRW.Value.c3.xyz = math.lerp(startPosition.Position, targetPosition, easeComponent.Ease.Evaluate(t));
        }
    }
}