using System.Runtime.CompilerServices;
using _src.Scripts.ZCollisions.ZCollision.Data;
using BovineLabs.Reaction.Data.Core;
using BovineLabs.Stats.Data;
using Drawing;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace _src.Scripts.ZCollisions.ZCollision.Editor
{
    [BurstCompile]
    [WithNone(typeof(CollisionEnterComponent))]
    public partial struct ZCollisionEditorJobEntity : IJobEntity
    {
        public CommandBuilder Drawing;
        public quaternion EditorCameraRotation;
        [ReadOnly] public NativeArray<(Entity entity, float3 position)>.ReadOnly Target;

        private static readonly Color BoxColor = Color.darkSeaGreen;
        private static readonly Color CollisionColor = Color.red;
        private const float LabelSize = 0.1f;
        private const float LabelLineHeight = LabelSize + 0.03f;

        [BurstCompile]
        private void Execute(in WorldRenderBounds worldRender)
        {
            var center = worldRender.Value.Center;
            int collisionCount = 0;
            var targetLength = Target.Length;
            for (var i = 0; i < targetLength; i++)
            {
                var (targetEntity, position) = Target[i];
                if (!worldRender.Value.Contains(position)) continue;
                Drawing.Line(center, position, CollisionColor);
                collisionCount++;
                var fraction = collisionCount / (float)targetLength;
                Drawing.Label3D(
                    collisionCount * LabelLineHeight, EditorCameraRotation,
                    $"{targetEntity.ToFixedString()}",
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