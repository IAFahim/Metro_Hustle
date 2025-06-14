#if ALINE
using _src.Scripts.Colliders.Colliders.Data;
using Drawing;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace _src.Scripts.WorldRenderCollisions.WorldRenderCollisions.Editor
{
    [BurstCompile]
    [WithAll(typeof(ColliderTag))]
    public partial struct WorldRenderEditorJobEntity : IJobEntity
    {
        public CommandBuilder Drawing;
        public quaternion EditorCameraRotation;

        private static readonly Color BoxColor = Color.darkSeaGreen;
        private static readonly Color CollisionColor = Color.red;
        private const float LabelSize = 0.1f;
        private const float LabelLineHeight = LabelSize + 0.03f;
        [ReadOnly] public NativeArray<TrackCollidableEntityBuffer>.ReadOnly CollisionTrackBuffer;
        [ReadOnly] public ComponentLookup<LocalToWorld> LookupLocalToWorld;


        [BurstCompile]
        private void Execute(in WorldRenderBounds worldRender)
        {
            var center = worldRender.Value.Center;
            int collisionCount = 0;
            var targetCount = CollisionTrackBuffer.Length;
            for (var i = 0; i < targetCount; i++)
            {
                var target = CollisionTrackBuffer[i].Entity;
                var position = LookupLocalToWorld[target].Position;
                if (!worldRender.Value.Contains(position)) continue;
                Drawing.Line(center, position, CollisionColor);
                collisionCount++;
                var fraction = collisionCount / (float)targetCount;
                Drawing.Label3D(
                    center - collisionCount * LabelLineHeight, EditorCameraRotation,
                    $"{target.ToFixedString()}",
                    LabelSize, CollisionColor
                );
                Drawing.WireBox(
                    center, worldRender.Value.Extents * (2 - fraction), BoxColor
                );
            }

            Drawing.WireBox(
                center, worldRender.Value.Extents * 2, BoxColor
            );
        }
    }
}
#endif