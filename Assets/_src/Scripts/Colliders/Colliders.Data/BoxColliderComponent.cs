using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Colliders.Colliders.Data
{
    public struct BoxColliderComponent : IComponentData
    {
        public half3 HalfExtents;
    }
}