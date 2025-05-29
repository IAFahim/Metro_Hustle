using Unity.Entities;

namespace _src.Scripts.Colliders.Colliders.Data
{
    [InternalBufferCapacity(0)]
    public partial struct CollisionEnterEntityBuffer : IBufferElementData
    {
        public Entity Entity;
    }
}