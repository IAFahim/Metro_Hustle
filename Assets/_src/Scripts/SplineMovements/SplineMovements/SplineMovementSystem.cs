using _src.Scripts.SplineMovements.SplineMovements.Data;
using ECSUnitySplineAddon.Runtime.Datas;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.SplineMovements.SplineMovements
{
    partial struct SplineMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var nativeSplineBlobComponentData = SystemAPI.GetSingleton<NativeSplineBlobComponentData>();
            ref var nativeSpline = ref nativeSplineBlobComponentData.Value.Value;
            var timeDeltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (
                         splineMoveComponent,
                         moveOffset,
                         localToWorld
                         )
                     in SystemAPI
                         .Query<
                             RefRW<SplineMoveComponent>,
                             RefRO<SplineSideOffsetComponent>,
                             RefRW<LocalToWorld>>()
                    )
            {
                var curveIndex = splineMoveComponent.ValueRO.CurveIndex;
                var distance = splineMoveComponent.ValueRO.Distance;
                var speed = splineMoveComponent.ValueRO.Speed * timeDeltaTime;
                nativeSpline.ToCurveT(curveIndex, distance + speed, out int index, out var newDistance, out var t);
                splineMoveComponent.ValueRW.CurveIndex = (byte)index;
                splineMoveComponent.ValueRW.Distance = (half)newDistance;

                nativeSpline.Evaluate(index, t, out float3 position, out var tangent, out var upVector);

                localToWorld.ValueRW.Value = float4x4.TRS(
                    position + localToWorld.ValueRO.Right * moveOffset.ValueRO.CurrentOffset,
                    quaternion.LookRotationSafe(tangent, upVector), 1);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}