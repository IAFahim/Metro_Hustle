using System.Runtime.CompilerServices;
using _src.Scripts.Colliders.Colliders.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.Colliders.Colliders.Editor
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor)]
    public partial struct BoxColliderEditorSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var a = new AABB();
            // var b = new AABB();
            // int c = 1;
            // foreach (var (localToWorld, worldRenderBounds) in SystemAPI
            //              .Query<RefRO<LocalToWorld>, RefRO<WorldRenderBounds>>())
            // {
            //     if (c == 1) a = worldRenderBounds.ValueRO.HalfExtents;
            //     if (c == 2) b = worldRenderBounds.ValueRO.HalfExtents;
            //     c++;
            // }

            var targetTLW = new LocalToWorld();
            TargetTrack targetTrack = new();
            foreach (var (localToWorld, targetLtwTag) in SystemAPI.Query<RefRO<LocalToWorld>, RefRO<TargetTrack>>())
            {
                targetTLW = localToWorld.ValueRO;
                targetTrack = targetLtwTag.ValueRO;
                break;
            }

#if ALINE
            var builder = Drawing.DrawingManager.GetBuilder();
            // Draw.WireBox(a.Center, a.Size);
            // Draw.WireBox(b.Center, b.Size);
            // Debug.Log(Overlaps(a, b));

            quaternion editorCamRot = quaternion.identity;
            if (UnityEditor.SceneView.lastActiveSceneView != null)
            {
                editorCamRot = UnityEditor.SceneView.lastActiveSceneView.camera.transform.rotation;
            }

            var boxColliderEditorAlineJobEntity = new BoxColliderEditorAlineJobEntity
            {
                Drawing = builder,
                TargetLTW = targetTLW,
                TargetTrack = targetTrack,
                EditorCameraRotation = editorCamRot
            };
            boxColliderEditorAlineJobEntity.ScheduleParallel();
            builder.DisposeAfter(state.Dependency);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Overlaps(AABB a, AABB b)
        {
            return math.all((a.Max >= b.Min) & (a.Min <= b.Max));
        }
    }
}