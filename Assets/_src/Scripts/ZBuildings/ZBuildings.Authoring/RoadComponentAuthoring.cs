﻿using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.ZBuildings.ZBuildings.Authoring
{
    internal class RoadComponentAuthoring : MonoBehaviour
    {
        public half sizeZ = new(20);
        public half sideGap = new(2);
        public half perLineWidth = new(2.5);
        public RoadFlag roadFlag = RoadFlag.Left1 | RoadFlag.Right1;

        private class ZRoadBaker : Baker<RoadComponentAuthoring>
        {
            public override void Bake(RoadComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var zRoadComponent = new RoadComponent
                {
                    SizeZ = authoring.sizeZ,
                    SideGap = authoring.sideGap,
                    RoadFlag = authoring.roadFlag,
                    PerLineWidth = authoring.perLineWidth,
                };
                AddComponent(entity, zRoadComponent);
            }
        }
    }
}