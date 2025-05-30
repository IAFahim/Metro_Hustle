// using _src.Scripts.Colliders.Colliders.Data;
// using _src.Scripts.InputControls.InputControls.Data;
// using _src.Scripts.ZBuildings.ZBuildings.Data;
// using _src.Scripts.ZMovements.ZMovements;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
//
// namespace _src.Scripts.RoadMovements.RoadMovements
// {
//     [BurstCompile]
//     public partial struct RoadLeftRightJobEntity : IJobEntity
//     {
//         public ComponentLookup<LocalToWorld> LtwLookup;
//         public ComponentLookup<ZMovementComponent> ZMovementLookup;
//         public ComponentLookup<DirectionInputEnableActiveComponent> DirectionInputEnableActiveLookup;
//         [ReadOnly] public NativeArray<CollisionTrackBuffer>.ReadOnly CollisionTrackBuffer;
//         private const float _allowedContact = 0.05f;
//
//         private void Execute(
//             in LocalToWorld roadLtw,
//             in RoadComponent roadComponent
//         )
//         {
//             var roadPositionZ = roadLtw.Position.z;
//             var roadComponentSizeZ = roadComponent.SizeZ;
//             foreach (var collisionTrack in CollisionTrackBuffer)
//             {
//                 var trackEntity = collisionTrack.Entity;
//                 var ltw = LtwLookup[trackEntity];
//                 if (math.abs(roadPositionZ - ltw.Position.z) < roadComponentSizeZ)
//                 {
//                     var zMovementComponent = ZMovementLookup[trackEntity];
//                     
//                 }
//
//             }
//         }
//     }
// }