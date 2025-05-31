using Unity.Entities;

namespace _src.Scripts.ZBuildings.ZBuildings.Data
{
    public struct BlockInfoComponent : IComponentData
    {
        public float PerBlockSize;
        public float CurrentCameraZ;
        public float PassedCameraZ;
    }
}