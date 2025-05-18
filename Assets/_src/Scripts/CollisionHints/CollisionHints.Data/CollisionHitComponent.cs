using _src.Scripts.CollisionHints.CollisionHints.Data.enums;
using Unity.Entities;

namespace _src.Scripts.CollisionHints.CollisionHints.Data
{
    public struct CollisionHitComponent : IComponentData
    {
        public CollisionHint Value;
    }
}