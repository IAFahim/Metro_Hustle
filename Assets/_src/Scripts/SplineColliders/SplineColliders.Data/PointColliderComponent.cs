using _src.Scripts.CollisionHints.CollisionHints.Data.enums;
using Unity.Entities;

namespace _src.Scripts.SplineColliders.SplineColliders.Data
{
    public struct PointColliderComponent : IComponentData
    {
        public CollisionFlag CollisionFlag;
    }
}