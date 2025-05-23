using _src.Scripts.SplineMovements.SplineMovements.Data;
using BovineLabs.Core;
using Drawing;
using ECSUnitySplineAddon.Runtime.Datas;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace ECSUnitySplineAddon.Editor
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    public partial struct SplineDebugSystem : ISystem
    {
        public SplineMoveComponent splineMoveComponent;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BLDebug>();
            state.RequireForUpdate<NativeSplineBlobComponentData>();
        }


        public void OnUpdate(ref SystemState state)
        {
            Entity splineEntity = SystemAPI.GetSingletonEntity<NativeSplineBlobComponentData>();
            NativeSplineBlobComponentData nativeSplineBlobComponentData =
                SystemAPI.GetComponent<NativeSplineBlobComponentData>(splineEntity);

            ref var splineBlob = ref nativeSplineBlobComponentData.Value.Value;
            var builder = DrawingManager.GetBuilder();
            splineMoveComponent = new SplineMoveComponent()
            {
                Speed = new(0.2),
                CurveIndex = 0,
                Distance = new half(0)
            };
            while (true)
            {
                splineBlob.ToCurveT(splineMoveComponent.CurveIndex,
                    splineMoveComponent.Distance + splineMoveComponent.Speed, out int index,
                    out float distance, out var t);
                splineBlob.Evaluate(index, t, out float3 position, out float3 tangent, out var upVector);
                builder.Cross(position);
                if (splineMoveComponent.CurveIndex == (byte)index &&
                    splineMoveComponent.Distance == (half)distance) break;
                splineMoveComponent.CurveIndex = (byte)index;
                splineMoveComponent.Distance = (half)distance;
            }

            builder.DisposeAfter(state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}