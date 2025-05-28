using Unity.Entities;

namespace _src.Scripts.ZCollisions.ZCollision.Data
{
    [InternalBufferCapacity(0)]
    public partial struct CollisionEnterEntityBuffer : IBufferElementData
    {
        public Entity Entity;
    }
}