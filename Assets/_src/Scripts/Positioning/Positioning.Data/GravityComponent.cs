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
        public byte GMultiplier;
        public half Velocity;
    }
}