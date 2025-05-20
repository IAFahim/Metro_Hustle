using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Colliders.Colliders.Data
{
    public struct ColliderRadiusSqComponent : IComponentData
    {
        public half RadiusSq;
    }
}