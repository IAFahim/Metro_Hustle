using Unity.Entities;

namespace _src.Scripts.Colliders.Colliders.Data
{
    [InternalBufferCapacity(0)]
    public struct CollisionTrackBuffer : IBufferElementData
    {
        public Entity Entity;
    }
}