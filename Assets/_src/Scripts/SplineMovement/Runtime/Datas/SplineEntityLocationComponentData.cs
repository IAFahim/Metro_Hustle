using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.SplineMovement.Runtime.Datas
{
    public struct SplineEntityLocationComponentData : IComponentData
    {
        public float3 Position;
        public quaternion LookRotationSafe;
    }
}