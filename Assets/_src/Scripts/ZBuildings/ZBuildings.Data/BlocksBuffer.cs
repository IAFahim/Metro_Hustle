using Unity.Entities;

namespace _src.Scripts.ZBuildings.ZBuildings.Data
{
    public struct BlockLeftBuffer : IBufferElementData
    {
        public Entity Entity;
    }
    
    public struct BlockRightBuffer : IBufferElementData
    {
        public Entity Entity;
    }
}