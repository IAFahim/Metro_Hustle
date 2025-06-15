using _src.Scripts.Easings.Runtime.Datas;
using _src.Scripts.Positioning.Positioning.Data;
using BovineLabs.Reaction.Data.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.Positioning.Positioning
{
    [WithPresent(typeof(StartPositionRecordComponent))]
    public partial struct EasePositioningJobEntity : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalToWorld> LtwLookup;

        private void Execute(
            ref LocalToWorld ltw,
            ref EaseComponent easeComponent,
            EnabledRefRW<StartPositionRecordComponent> startPositionRecorded,
            ref StartPositionRecordComponent startPosition,
            in Targets targets
        )
        {
            if (startPositionRecorded.ValueRO == false)
            {
                startPosition.Position = ltw.Position;
                startPositionRecorded.ValueRW = true;
            }

            var position = LtwLookup[targets.Target].Position;
        }
    }
}