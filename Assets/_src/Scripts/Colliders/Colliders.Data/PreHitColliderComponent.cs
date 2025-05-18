using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Colliders.Colliders.Data
{
    public struct PreHitColliderComponent : IComponentData
    {
        public half Forward;
        public half RadiusSq;
    }
}