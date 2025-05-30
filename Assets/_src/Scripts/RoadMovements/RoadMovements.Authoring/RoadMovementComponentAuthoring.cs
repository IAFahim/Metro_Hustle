using _src.Scripts.RoadMovements.RoadMovements.Data;
using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.RoadMovements.RoadMovements.Authoring
{
    internal class RoadMovementComponentAuthoring : MonoBehaviour
    {
        public RoadFlag roadFlagCurrentBit= RoadFlag.Center;

        private class RoadMoveComponentBaker : Baker<RoadMovementComponentAuthoring>
        {
            public override void Bake(RoadMovementComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new RoadMovementComponent
                {
                    CurrentRoadFlag = authoring.roadFlagCurrentBit
                });
            }
        }
    }
}