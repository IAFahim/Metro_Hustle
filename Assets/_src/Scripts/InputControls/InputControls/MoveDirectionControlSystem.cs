// using _src.Scripts.InputControls.InputControls.Data;
// using BovineLabs.Core.Input;
// using Unity.Burst;
// using Unity.Entities;
// using Unity.Mathematics;
// using UnityEngine;
//
// namespace _src.Scripts.InputControls.InputControls
// {
//     [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
//     public partial struct MoveDirectionControlSystem : ISystem
//     {
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//         }
//
//
//         public void OnUpdate(ref SystemState state)
//         {
//             var inputComponentEntity = SystemAPI.GetSingletonEntity<InputComponent>();
//             var inputComponent = SystemAPI.GetComponent<InputComponent>(inputComponentEntity);
//             var threshold = SystemAPI.GetComponent<TouchInputThresholdSingleton>(inputComponentEntity);
//             var moveDelta = inputComponent.MoveDelta;
//
//             var absDelta = math.abs(moveDelta);
//             var isUpDown = absDelta.y > absDelta.x;
//             var upInput = isUpDown && moveDelta.y > 0;
//             var downInput = isUpDown && moveDelta.y < 0;
//             var leftInput = !isUpDown && moveDelta.x < 0;
//             var rightInput = !isUpDown && 0 < moveDelta.x;
//             var inputLive = math.lengthsq(inputComponent.MoveDelta) >0;
//
//             MoveDirectionIJobEntity moveDirectionIJobEntity = new MoveDirectionIJobEntity()
//             {
//                 InputLive = inputLive.x | inputLive.y,
//                 UpInput = upInput,
//                 DownInput = downInput,
//                 LeftInput = leftInput,
//                 RightInput = rightInput
//             };
//             moveDirectionIJobEntity.ScheduleParallel();
//         }
//
//         [BurstCompile]
//         public void OnDestroy(ref SystemState state)
//         {
//         }
//     }
// }