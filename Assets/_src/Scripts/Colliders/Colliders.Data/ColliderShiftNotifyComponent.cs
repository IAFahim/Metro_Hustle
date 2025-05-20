using _src.Scripts.Collisions.Collisions.Data.enums;
using Unity.Entities;

namespace _src.Scripts.Colliders.Colliders.Data
{
    public struct ColliderShiftNotifyComponent : IComponentData
    {
        public CollisionHint CollisionHint;
        public CollisionEffect CollisionEffect;
    }
}