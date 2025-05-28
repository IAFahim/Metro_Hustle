#if ALINE

using _src.Scripts.ZBuildings.ZBuildings.Data;
using Drawing;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace _src.Scripts.ZBuildings.ZBuildings.Editor
{
    public partial struct ZBuildingEditorJobEntity : IJobEntity
    {
        public CommandBuilder Drawing;
        private static readonly Color StartColor = Color.lightBlue;
        private static readonly Color EndColor = Color.cornflowerBlue;

        private void Execute(in BuildingGapComponent buildingGap, in WorldRenderBounds worldRenderBounds)
        {
            var center = worldRenderBounds.Value.Center;
            var extents = worldRenderBounds.Value.Extents;
            var externZ = new float3(0, 0, extents.z);
            var forward = center + externZ;
            var backward = center - externZ;
            var start = center - new float3(0, 0, buildingGap.Start);
            var end = center + new float3(0, 0, buildingGap.End);
            Drawing.Arrow(center, start, StartColor);
            Drawing.Arrow(center, end, EndColor);
        }
    }
}
#endif