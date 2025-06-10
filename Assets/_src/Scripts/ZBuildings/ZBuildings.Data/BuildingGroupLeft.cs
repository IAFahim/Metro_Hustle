using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.ZBuildings.ZBuildings.Data
{
    public struct BuildingGroupLeft : IBufferElementData
    {
        public half StartOffset;
        public Entity Prefab;
    }
}