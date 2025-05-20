using _src.Scripts.Colliders.Colliders.Data.enums;
using Unity.Entities;

namespace _src.Scripts.Colliders.Colliders.Data
{
    public struct ColliderFlagComponent : IComponentData
    {
        public ColliderFlag ColliderFlag;
    }
}