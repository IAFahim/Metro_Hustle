using _src.Scripts.CollisionHints.CollisionHints.Data.enums;
using Unity.Entities;

namespace _src.Scripts.CollisionHints.CollisionHints.Data.Datas
{
    public struct PreCollisionData
    {
        public Entity Entity;
        public PreCollisionHint PreCollisionHint;
    }
}