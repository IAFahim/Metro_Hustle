using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Colliders.Colliders.Data
{
    public struct SphereColliderComponent : IComponentData
    {
        public float LengthSq;
        public half DestroySq;
    }
}