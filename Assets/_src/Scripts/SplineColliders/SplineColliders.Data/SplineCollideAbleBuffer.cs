using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.SplineColliders.SplineColliders.Data
{
    [BurstCompile]
    public struct SplineCollideAbleBuffer : IBufferElementData
    {
        public Entity Entity;
        public byte SplineLine;
        public byte CollisionEvent;
        public float3 Position;
        
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool InSphere(float3 colliderOrigin, float3 offset, float radiusSq)
        {
            var spherePosition = colliderOrigin + offset;
            var distance = math.distancesq(spherePosition, Position);
            return distance < radiusSq;
        }
    }
}