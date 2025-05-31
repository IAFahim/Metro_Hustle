using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.ZBuildings.ZBuildings.Data
{
    public struct BlockCreateLogicComponent : IComponentData
    {
        public bool PreWarmed;
        public half PerBlockSize;
        public half Progress;
        public half AHeadCreate;
        public half SideOffset;
        public ushort CreateCount;
        public float PassedCameraZ;
    }
}