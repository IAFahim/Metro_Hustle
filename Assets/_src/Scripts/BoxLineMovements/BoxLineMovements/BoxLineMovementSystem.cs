using System.Runtime.CompilerServices;
using Drawing;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace _src.Scripts.BoxLineMovements.BoxLineMovements
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    public partial struct BoxLineMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            
        }


        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(AABB obj, AABB aabb)
        {
            return math.all(obj.Max >= aabb.Min & obj.Min <= aabb.Max);
        }
    }
}