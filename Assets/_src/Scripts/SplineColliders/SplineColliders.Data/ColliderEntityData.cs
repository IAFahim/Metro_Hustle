using _src.Scripts.SplineConfigs.SplineConfigs.Data;
using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.SplineColliders.SplineColliders.Data
{
    public struct ColliderEntityData
    {
        public Entity Entity;
        public float3 Position;
        public SplineLineComponent SplineLineComponent;
    }
}