using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Colliders.Colliders.Data
{
    public struct ColliderPreHitComponent : IComponentData
    {
        public half Forward;
        public half RadiusSq;
    }
}