using _src.Scripts.Collisions.Collisions.Data.enums;
using Unity.Entities;

namespace _src.Scripts.Collisions.Collisions.Data
{
    public struct CollisionComponent : IComponentData
    {
        public CollisionHint Hint;
        public CollisionEffect Effect;
    }
}