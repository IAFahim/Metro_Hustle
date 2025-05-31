using _src.Scripts.Colliders.Colliders.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.WorldRenderCollisions.WorldRenderCollisions.Editor
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    public partial struct WorldRenderCollisionEditorSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CollisionTrackBuffer>();
        }

        public void OnUpdate(ref SystemState state)
        {
#if ALINE
            quaternion editorCamRot = quaternion.identity;
            if (UnityEditor.SceneView.lastActiveSceneView != null)
            {
                editorCamRot = UnityEditor.SceneView.lastActiveSceneView.camera.transform.rotation;
            }

            var builder = Drawing.DrawingManager.GetBuilder();
            var zCollisionEditorJobEntity = new WorldRenderEditorJobEntity()
            {
                Drawing = builder,
                EditorCameraRotation = editorCamRot,
                CollisionTrackBuffer =
                    SystemAPI.GetSingletonBuffer<CollisionTrackBuffer>().AsNativeArray().AsReadOnly(),
                LookupLocalToWorld = SystemAPI.GetComponentLookup<LocalToWorld>(true)
            };
            zCollisionEditorJobEntity.Schedule();
            builder.DisposeAfter(state.Dependency);
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}