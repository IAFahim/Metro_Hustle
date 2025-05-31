using Unity.Entities;

namespace _src.Scripts.ZBuildings.ZBuildings.Data
{
    public struct BlockBuffer : IBufferElementData
    {
        public Entity Left;
        public Entity Road;
        public Entity Right;
    }
}