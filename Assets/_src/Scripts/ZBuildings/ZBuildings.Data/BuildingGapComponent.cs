using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.ZBuildings.ZBuildings.Data
{
    public struct BuildingGapComponent : IComponentData
    {
        public half Start;
        public half End;
    }
}
