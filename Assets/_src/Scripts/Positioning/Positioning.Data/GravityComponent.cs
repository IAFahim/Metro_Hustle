using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Positioning.Positioning.Data
{
    [BurstCompile]
    public struct GravityComponent : IComponentData
    {
        public half Gravity;
        public half GravityMul;
        public half Velocity;

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GravityComponent DisableFalling() =>
            new()
            {
                Gravity = Gravity,
                GravityMul = new half(0),
                Velocity = new half(0),
            };

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GravityComponent EnableJump( half velocity) =>
            new()
            {
                Gravity = Gravity,
                GravityMul = new half(1),
                Velocity = velocity,
            };
    }
}