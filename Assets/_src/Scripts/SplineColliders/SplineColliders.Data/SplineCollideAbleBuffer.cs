using System.Runtime.CompilerServices;
using _src.Scripts.SplineConfigs.SplineConfigs.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.SplineColliders.SplineColliders.Data
{
    [BurstCompile]
    public struct SplineCollideAbleBuffer : IBufferElementData
    {
        public Entity Entity;
        public float3 Position;
        public SplineLineComponent SplineLineComponent;
        
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