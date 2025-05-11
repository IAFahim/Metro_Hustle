using Drawing;
using ECSUnitySplineAddon.Runtime.Datas;
using Unity.Burst;
using Unity.Entities;

namespace ECSUnitySplineAddon.Editor
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.Editor|WorldSystemFilterFlags.Default)]
    public partial struct SplineDebugSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NativeSplineBlobComponentData>();
        }

        [BurstCompile]
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
            
            
            // ref var curves = ref nativeSplineBlobComponentData.Value.Value.;
            // ref var curves = ref nativeSplineBlobComponentData.Value.Value.EvaluatePosition();

            
            builder.DisposeAfter(state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}