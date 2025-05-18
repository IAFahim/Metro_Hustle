using System.Runtime.CompilerServices;
using _src.Scripts.Colliders.Colliders.Data.enums;
using _src.Scripts.Collisions.Collisions.Data.enums;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.SplineColliders.SplineColliders.Data
{
    [BurstCompile]
    public struct SplinePointColliderBuffer : IBufferElementData
    {
        public Entity Entity;
        public byte SplineLine;
        public ColliderFlag ColliderFlag;
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