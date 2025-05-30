// using _src.Scripts.Colliders.Colliders.Data;
// using Unity.Burst;
// using Unity.Entities;
// using Unity.Transforms;
//
// namespace _src.Scripts.RoadMovements.RoadMovements
// {
//     public partial struct RoadLeftRightSystem : ISystem
//     {
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             
//         }
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             var roadLeftRightJobEntity = new RoadLeftRightJobEntity()
//             {
//                 CollisionTrackBuffer = SystemAPI.GetSingletonBuffer<CollisionTrackBuffer>().AsNativeArray().AsReadOnly(),
//                 LtwLookup = SystemAPI.GetComponentLookup<LocalToWorld>()
//             };
//             roadLeftRightJobEntity.ScheduleParallel();
//         }
//
//         [BurstCompile]
//         public void OnDestroy(ref SystemState state)
//         {
//
//         }
//     }
// }