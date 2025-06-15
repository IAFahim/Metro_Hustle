using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Positioning.Positioning.Data
{
    [BurstCompile]
    public struct LeftRightComponent : IComponentData
    {
        public half Speed;
        public float Target;

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int GetDirection(float current)
        {
            if (Hint.Likely(current == Target)) return 0;
            return (current < Target) ? 1 : -1;
        }
    }
}