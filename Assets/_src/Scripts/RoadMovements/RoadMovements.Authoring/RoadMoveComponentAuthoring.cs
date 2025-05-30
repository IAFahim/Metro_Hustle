using _src.Scripts.RoadMovements.RoadMovements.Data;
using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.RoadMovements.RoadMovements.Authoring
{
    internal class RoadMoveComponentAuthoring : MonoBehaviour
    {
        public RoadFlag roadFlagCurrentBit= RoadFlag.Center;

        private class RoadMoveComponentBaker : Baker<RoadMoveComponentAuthoring>
        {
            public override void Bake(RoadMoveComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new RoadMoveComponent
                {
                    RoadFlagCurrentBit = authoring.roadFlagCurrentBit
                });
            }
        }
    }
}