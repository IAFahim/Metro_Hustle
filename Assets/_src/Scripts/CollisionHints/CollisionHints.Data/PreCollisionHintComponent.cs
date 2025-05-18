using _src.Scripts.CollisionHints.CollisionHints.Data.enums;
using Unity.Entities;

namespace _src.Scripts.CollisionHints.CollisionHints.Data
{
    public struct PreCollisionHintComponent : IComponentData
    {
        public PreCollisionHint Value;
    }
}