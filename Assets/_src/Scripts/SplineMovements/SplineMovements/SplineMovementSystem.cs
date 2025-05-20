using _src.Scripts.SplineMovements.SplineMovements.Data;
using ECSUnitySplineAddon.Runtime;
using ECSUnitySplineAddon.Runtime.Datas;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace _src.Scripts.SplineMovements.SplineMovements
{
    partial struct SplineMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var nativeSplineBlobComponentData = SystemAPI.GetSingleton<NativeSplineBlobComponentData>();
            ref var nativeSpline = ref nativeSplineBlobComponentData.Value.Value;
            var timeDeltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (splineMoveComponent, localTransform, localToWorld) in SystemAPI.Query<RefRW<SplineMoveComponent>, RefRW<LocalTransform>, RefRO<LocalToWorld>>())
            {
                var curveIndex = splineMoveComponent.ValueRO.CurveIndex;
                var distance = splineMoveComponent.ValueRO.Distance;
                var speed = splineMoveComponent.ValueRO.Speed * timeDeltaTime;
                nativeSpline.ToCurveT(curveIndex, distance + speed, out int index, out var newDistance, out float t);
                splineMoveComponent.ValueRW.CurveIndex = (byte)index;
                splineMoveComponent.ValueRW.Distance = (half)newDistance;
                
                nativeSpline.Evaluate(index, t, out float3 position, out var tangent, out var upVector );
                localTransform.ValueRW.Position = position + localToWorld.ValueRO.Right * splineMoveComponent.ValueRO.SideOffset;
                localTransform.ValueRW.Rotation = quaternion.LookRotationSafe(tangent, upVector);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}