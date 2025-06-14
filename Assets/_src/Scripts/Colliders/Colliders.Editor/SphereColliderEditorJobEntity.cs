#if ALINE
using _src.Scripts.Colliders.Colliders.Data;
using _src.Scripts.Positioning.Positioning.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Editor
{
    [BurstCompile]
    public partial struct SphereColliderEditorJobEntity : IJobEntity
    {
        [ReadOnly] public NativeArray<TrackCollidableEntityBuffer>.ReadOnly TrackCollidableEntityBuffer;
        [ReadOnly] public ComponentLookup<LocalToWorld> LtwLookup;

        [BurstCompile]
        private void Execute(in LocalToWorld ltw, in SphereColliderComponent colliderComponent, ref LeftRightComponent leftRightComponent)
        {
            foreach (var entityBuffer in TrackCollidableEntityBuffer)
            {
                var targetPosition = LtwLookup[entityBuffer.Entity].Position;
                var difference = targetPosition - ltw.Position;
                var distance = math.lengthsq(difference);
                if (distance < colliderComponent.LengthSqrt)
                {
                    leftRightComponent.Target = (half)targetPosition.x;
                }
            }
        }
    }
}
#endif