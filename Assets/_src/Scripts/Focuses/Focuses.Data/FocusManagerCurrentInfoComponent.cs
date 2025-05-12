using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Focuses.Focuses.Data
{
    public struct FocusManagerCurrentInfoComponent : IComponentData
    {
        public float3 Position;
        public quaternion Rotation;
    }
}