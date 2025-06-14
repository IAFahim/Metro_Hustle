using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Positioning.Positioning.Data
{
    [BurstCompile]
    public struct LeftRightComponent : IComponentData
    {
        public half Speed;
        public half Current;
        public half Target;

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int GetDirection()
        {
            if (Current == Target) return 0;
            return (Current < Target) ? 1 : -1;
        }
    }
}