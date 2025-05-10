// using BovineLabs.Core.Input;
// using ECS_Spline.Runtime.Datas;
// using ECSUnitySplineAddon.Runtime.Datas;
// using Unity.Burst;
// using Unity.Entities;
// using Unity.Transforms;
//
// namespace _src.Scripts.SplineMovement.Runtime.Systems
// {
//     [BurstCompile]
//     public partial struct PlayerLineMovementSystem : ISystem
//     {
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<NativeSplineBlobComponentData>();
//             state.RequireForUpdate<InputCoreComponentData>();
//         }
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             var splineEntity = SystemAPI.GetSingletonEntity<NativeSplineBlobComponentData>();
//             var splineForwardMovementJobEntity = new SplineForwardMovementJobEntity
//             {
//                 TimeDelta = SystemAPI.Time.DeltaTime,
//                 SplineLocalTransform = SystemAPI.GetComponent<LocalTransform>(splineEntity),
//                 NativeSplineBlob = SystemAPI.GetComponent<NativeSplineBlobComponentData>(splineEntity).Value,
//             };
//             splineForwardMovementJobEntity.ScheduleParallel();
//         }
//
//         [BurstCompile]
//         public void OnDestroy(ref SystemState state)
//         {
//         }
//     }
// }