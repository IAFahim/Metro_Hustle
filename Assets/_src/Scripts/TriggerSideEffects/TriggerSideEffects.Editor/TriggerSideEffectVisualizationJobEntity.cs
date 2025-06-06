#if ALINE
using _src.Scripts.TriggerSideEffects.TriggerSideEffects.Data;
using Drawing;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects.Editor
{
    [BurstCompile]
    [WithAll(typeof(TriggerSideEffectComponent))]
    public partial struct TriggerSideEffectVisualizationJobEntity : IJobEntity
    {
        public CommandBuilder Drawing;
        public quaternion EditorCameraRotation;

        // Colors for different trigger types
        private static readonly Color TriggerBoundsColor = Color.cyan;
        private static readonly Color InfoTextColor = Color.white;

        private const float LabelSize = 0.08f;

        [BurstCompile]
        private void Execute(
            in WorldRenderBounds worldRender,
            in TriggerSideEffectComponent triggerSideEffect
        )
        {
            var center = worldRender.Value.Center;
            var extents = worldRender.Value.Extents;
            DrawTriggerInfo(center, extents, triggerSideEffect);
        }

        private void DrawTriggerInfo(float3 center, float3 extents, in TriggerSideEffectComponent trigger)
        {
            var labelPos = center + new float3(0, extents.y + 0.3f, 0);

            Drawing.Label3D(labelPos, EditorCameraRotation,
                $"Flags: {trigger.TriggerType}", LabelSize * 0.7f, InfoTextColor);
        }
    }
}
#endif