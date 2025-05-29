using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.ZBuildings.ZBuildings.Authoring
{
    internal class ZRoadComponentAuthoring : MonoBehaviour
    {
        public half sizeZ = new(20);
        public half sideGap = new(2);
        public half perLineWidth = new(2.5);
        public RoadFlag roadFlag = RoadFlag.Left1 |  RoadFlag.Right1;

        private class ZRoadBaker : Baker<ZRoadComponentAuthoring>
        {
            public override void Bake(ZRoadComponentAuthoring componentAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ZRoadComponent
                {
                    SizeZ = componentAuthoring.sizeZ,
                    SideGap = componentAuthoring.sideGap,
                    RoadFlag = componentAuthoring.roadFlag,
                    PerLineWidth = componentAuthoring.perLineWidth
                });
            }
        }
    }
}