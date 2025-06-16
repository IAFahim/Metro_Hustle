using Unity.Entities;

namespace _src.Scripts.ZBuildings.ZBuildings.Data
{
    public struct StationData : IComponentData
    {
        public Entity StartBlock;
        public Entity StartRoad;
        public Entity EndBlock;
        public Entity EndRoad;
    }
}