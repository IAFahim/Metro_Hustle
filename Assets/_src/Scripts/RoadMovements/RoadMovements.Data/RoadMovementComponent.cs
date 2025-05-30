using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Entities;

namespace _src.Scripts.RoadMovements.RoadMovements.Data
{
    public struct RoadMovementComponent : IComponentData
    {
        public RoadFlag RoadFlagCurrentBit;
    }
}