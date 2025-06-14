using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;


namespace _src.Scripts.ZBuildings.ZBuildings.Editor // Or your preferred editor namespace
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(PresentationSystemGroup))] // Or another suitable editor update group
    public partial struct ZRoadVisualizerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // No specific creation needed for this simple visualizer
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
#if ALINE
            // Ensure there's at least one RoadComponent to avoid unnecessary builder creation
            // This is a micro-optimization, can be skipped if preferred.
            var query = SystemAPI.QueryBuilder().WithAll<RoadComponent, LocalToWorld>().Build();
            if (query.IsEmpty)
            {
                return;
            }
            
            var builder = Drawing.DrawingManager.GetBuilder();


            // Optional: Pass editor camera rotation if you plan to add labels that need to face the camera
            // quaternion editorCamRot = quaternion.identity;
            // if (UnityEditor.SceneView.lastActiveSceneView != null)
            // {
            //    editorCamRot = UnityEditor.SceneView.lastActiveSceneView.camera.transform.rotation;
            // }

            var job = new ZRoadVisualizerJob
            {
                Drawing = builder
                // EditorCameraRotation = editorCamRot // If using labels
            };

            state.Dependency = job.ScheduleParallel(query, state.Dependency); // Schedule with query
            builder.DisposeAfter(state.Dependency);
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}