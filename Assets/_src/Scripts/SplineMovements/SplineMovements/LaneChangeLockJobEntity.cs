using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.enums;
using _src.Scripts.SplineMovements.SplineMovements.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.SplineMovements.SplineMovements
{
    public partial struct LaneChangeLockJobEntity : IJobEntity
    {
        public float TimeDelta;

        private void Execute(
            ref DirectionLockComponent directionLockComponent,
            ref SplineLineComponent splineLineComponent,
            in DirectionInputEnableActiveComponent directionInputEnableActiveComponent,
            ref SplineSideOffsetComponent splineSideOffsetComponent
        )
        {
            byte maxLane = 3;
            float gap = new half(1.5);


            if (splineSideOffsetComponent.Moving)
            {
                if (
                    math.abs(splineSideOffsetComponent.CurrentOffset - splineSideOffsetComponent.EndOffset) > 0.05
                )
                {
                    var step = TimeDelta * splineSideOffsetComponent.Speed;
                    splineSideOffsetComponent.CurrentOffset =
                        (half)math.lerp(splineSideOffsetComponent.CurrentOffset, splineSideOffsetComponent.EndOffset,
                            step);
                }
                else
                {
                    var isRight = splineSideOffsetComponent.CurrentOffset < splineSideOffsetComponent.EndOffset;
                    if (isRight) directionLockComponent.Flag &= ~DirectionLockFlag.LockRightOnly;
                    else directionLockComponent.Flag &= ~DirectionLockFlag.LockLeftOnly;
                    splineSideOffsetComponent.CurrentOffset = splineSideOffsetComponent.EndOffset;
                    splineSideOffsetComponent.Moving = false;
                }
            }
            else
            {
                if ((directionInputEnableActiveComponent.Flag & DirectionEnableActiveFlag.IsLeftEnabledAndActive) ==
                    DirectionEnableActiveFlag.IsLeftEnabledAndActive && splineLineComponent.Line != 0)
                {
                    splineSideOffsetComponent.Moving = true;
                    splineLineComponent.Value--;
                    directionLockComponent.Flag |= DirectionLockFlag.LockLeftOnly;
                    splineSideOffsetComponent.EndOffset = (half)(splineSideOffsetComponent.CurrentOffset - gap);
                    Debug.Log("Left");
                }

                if ((directionInputEnableActiveComponent.Flag & DirectionEnableActiveFlag.IsRightEnabledAndActive) ==
                    DirectionEnableActiveFlag.IsRightEnabledAndActive &
                    splineLineComponent.Line != 2)
                {
                    splineSideOffsetComponent.Moving = true;
                    splineLineComponent.Value++;
                    directionLockComponent.Flag |= DirectionLockFlag.LockRightOnly;
                    splineSideOffsetComponent.EndOffset = (half)(splineSideOffsetComponent.CurrentOffset + gap);
                    Debug.Log("Right");
                }
            }
        }
    }
}