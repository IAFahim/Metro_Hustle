// using _src.Scripts.InputControls.InputControls.Data;
// using _src.Scripts.InputControls.InputControls.Data.enums;
// using _src.Scripts.SplineMovements.SplineMovements.Data;
// using Unity.Entities;
// using Unity.Mathematics;
//
// namespace _src.Scripts.SplineMovements.SplineMovements
// {
//     public partial struct LaneChangeLockJobEntity : IJobEntity
//     {
//         public float TimeDelta;
//
//         public void Execute(
//             ref DirectionLockComponent directionLockComponent,
//             ref SplineLineComponent splineLineComponent,
//             in DirectionInputEnableActiveComponent directionInputEnableActiveComponent,
//             ref SplineSideOffsetComponent splineSideOffsetComponent
//         )
//         {
//             byte maxLane = 3;
//             half gap = new half(1.5);
//             // -2 (end) < 0 (start)
//             // if (splineSideOffsetComponent.EndOffset < splineSideOffsetComponent.CurrentOffset)
//             // {
//             //     directionLockComponent.Flag = DirectionLockFlag.LockLeftOnly;
//             //     return;
//             // }
//             // // 2 (end) < 0 (start)
//             // if (splineSideOffsetComponent.EndOffset > splineSideOffsetComponent.CurrentOffset)
//             // {
//             //     directionLockComponent.Flag = DirectionLockFlag.LockRightOnly;
//             //     return;
//             // }
//
//             var isLeft = 
//
//             if (math.abs(splineSideOffsetComponent.CurrentOffset - splineSideOffsetComponent.EndOffset) < 0.05f)
//             {
//                 var step = TimeDelta * splineSideOffsetComponent.Speed;
//                 splineSideOffsetComponent.CurrentOffset =
//                     (half)math.lerp(splineSideOffsetComponent.CurrentOffset, splineSideOffsetComponent.EndOffset, step);
//             }
//             else if (splineSideOffsetComponent.Moving)
//             {
//                 splineSideOffsetComponent.CurrentOffset = splineSideOffsetComponent.EndOffset;
//                 directionLockComponent.Flag &= ~DirectionLockFlag.LockLeftOnly;
//                 directionLockComponent.Flag &= ~DirectionLockFlag.LockRightOnly;
//                 
//             }
//         }
//     }
// }