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
        public RoadFlag roadFlag = RoadFlag.Left1 | RoadFlag.Right1;
        public float roadTriggerHeight = 0.1f;

        private class ZRoadBaker : Baker<ZRoadComponentAuthoring>
        {
            public override void Bake(ZRoadComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var zRoadComponent = new ZRoadComponent
                {
                    SideGap = authoring.sideGap,
                    RoadFlag = authoring.roadFlag,
                    PerLineWidth = authoring.perLineWidth,
                };
                zRoadComponent.Extents =
                    zRoadComponent.CalculateExtern(authoring.roadTriggerHeight, authoring.sizeZ, false);
                AddComponent(entity, zRoadComponent);
            }
        }
    }
}