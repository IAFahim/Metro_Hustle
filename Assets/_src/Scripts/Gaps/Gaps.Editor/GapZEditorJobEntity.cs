#if ALINE

using _src.Scripts.Gaps.Gaps.Data;
using Drawing;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace _src.Scripts.Gaps.Gaps.Editor
{
    public partial struct GapZEditorJobEntity : IJobEntity
    {
        public CommandBuilder Drawing;
        private static readonly Color ForwardColor = Color.lightBlue;
        private static readonly Color BackwardColor = Color.cornflowerBlue;

        private void Execute(in GapZComponent gapZ, in WorldRenderBounds worldRenderBounds)
        {
            var center = worldRenderBounds.Value.Center;
            var extents = worldRenderBounds.Value.Extents;
            var externZ = new float3(0, 0, extents.z);
            var renderForward = center + externZ;
            var renderBackward = center - externZ;

            var worldForward = renderForward + new float3(0, 0, gapZ.Forward);
            var worldBackward = renderBackward - new float3(0, 0, gapZ.Backward);
            Drawing.Arrow(renderForward, worldForward, ForwardColor);
            Drawing.Arrow(renderBackward, worldBackward, BackwardColor);
        }
    }
}
#endif