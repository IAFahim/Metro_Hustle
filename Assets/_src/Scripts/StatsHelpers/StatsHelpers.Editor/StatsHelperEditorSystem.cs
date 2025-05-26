// using BovineLabs.Stats.Data;
// using Unity.Burst;
// using Unity.Entities;
// using Unity.Mathematics;
//
// namespace _src.Scripts.StatsHelpers.StatsHelpers.Editor
// {
//     [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
//     public partial struct StatsHelperEditorSystem : ISystem
//     {
//         private BufferLookup<Stat> _bufferLookup;
//         
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             _bufferLookup = state.GetBufferLookup<Stat>(true);
//         }
//         
//         public void OnUpdate(ref SystemState state)
//         {
// #if ALINE
//             _bufferLookup.Update(ref state);
//             var builder = Drawing.DrawingManager.GetBuilder();
//             quaternion editorCamRot = quaternion.identity;
//             if (UnityEditor.SceneView.lastActiveSceneView != null)
//             {
//                 editorCamRot = UnityEditor.SceneView.lastActiveSceneView.camera.transform.rotation;
//             }
//
//             SystemAPI.GetBufferLookup<Stat>();
//             var statsHelperEditorJobEntity = new StatsHelperEditorJobEntity()
//             {
//                 Drawing = builder,
//                 StatsBuffer = _bufferLookup
//                     
//             };
//             statsHelperEditorJobEntity.ScheduleParallel();
//             builder.DisposeAfter(state.Dependency);
// #endif
//         }
//
//         [BurstCompile]
//         public void OnDestroy(ref SystemState state)
//         {
//
//         }
//     }
// }