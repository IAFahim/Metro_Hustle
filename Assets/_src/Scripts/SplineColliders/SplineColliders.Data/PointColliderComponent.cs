using _src.Scripts.Colliders.Colliders.Data.enums;
using Unity.Entities;

namespace _src.Scripts.SplineColliders.SplineColliders.Data
{
    public struct PointColliderComponent : IComponentData
    {
        public ColliderFlag ColliderFlag;
    }
}