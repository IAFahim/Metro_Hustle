using BovineLabs.Core;
using Drawing;
using ECSSplines.Runtime;
using ECSUnitySplineAddon.Runtime.Datas;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSUnitySplineAddon.Editor
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    public partial struct SplineDebugSystem : ISystem
    {
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

            ref var curves = ref nativeSplineBlobComponentData.Value.Value.Curves;
            var builder = DrawingManager.GetBuilder();
            for (int i = 0; i < curves.Length; i++)
            {
                var bezierCurve = curves[i];
                builder.Bezier(bezierCurve.P0, bezierCurve.P1, bezierCurve.P2, bezierCurve.P3);
            }

            // var debug = SystemAPI.GetSingleton<BLDebug>();
            ref var distanceLut = ref nativeSplineBlobComponentData.Value.Value.DistanceLUT;
            ref var length = ref nativeSplineBlobComponentData.Value.Value.Length;
            var lutResolution = NativeSplineBlobFactory.LUT_RESOLUTION;
            for (int i = 0, seg = 0; i < distanceLut.Length; i++, seg++)
            {
                if (seg == lutResolution) seg = -1;
                var t = distanceLut[i].Distance / length;
                var position = nativeSplineBlobComponentData.Value.Value.EvaluatePosition(t);
                builder.Cross(position);
            }
            // ref var curves = ref nativeSplineBlobComponentData.Value.Value.EvaluatePosition();


            builder.DisposeAfter(state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}