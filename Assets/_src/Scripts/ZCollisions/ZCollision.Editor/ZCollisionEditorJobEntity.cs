using Drawing;
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace _src.Scripts.ZCollisions.ZCollision.Editor
{
    [BurstCompile]
    public partial struct ZCollisionEditorJobEntity : IJobEntity
    {
        public CommandBuilder Drawing;
        public static readonly Color BoxColor = Color.darkSeaGreen;

        private void Execute(in WorldRenderBounds worldRender)
        {
            Drawing.WireBox(worldRender.Value.Center, worldRender.Value.Extents * 2, BoxColor);
        }
    }
}