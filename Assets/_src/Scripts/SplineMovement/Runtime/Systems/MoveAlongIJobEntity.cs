
using _src.Scripts.SplineMovement.Runtime.Datas;
using ECSUnitySplineAddon.Runtime.Datas;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.SplineMovement.Runtime.Systems
{
    [BurstCompile]
    public partial struct MoveAlongIJobEntity : IJobEntity
    {
        [ReadOnly] public float TimeDelta;
        [ReadOnly] public LocalToWorld SplineLocalToWorld;
        [ReadOnly] public BlobAssetReference<NativeSplineBlob> SplineBlob;


        [BurstCompile]
        private void Execute(
            ref SplineLinkComponentData splineLink,
            ref SplineEntityLocationComponentData splineEntityLocation
        )
        {
            float curveLength = SplineBlob.Value.GetCurveLength(splineLink.CurveIndex);
            float offsetThisFrame =  TimeDelta;
            splineLink.DistanceInCurve += offsetThisFrame;
            splineLink.TraveledDistance += math.abs(offsetThisFrame);
            float normalizedT = (splineLink.DistanceInCurve + splineLink.DistanceOffset) / curveLength;

            SplineBlob.Value.Evaluate(splineLink.CurveIndex, normalizedT, out float3 localPosition,
                out float3 localTangent, out float3 localUpVector);

            float3 worldPosition = math.transform(SplineLocalToWorld.Value, localPosition);
            float3 worldTangent = math.rotate(SplineLocalToWorld.Value, localTangent);
            float3 worldUpVector = math.rotate(SplineLocalToWorld.Value, localUpVector);

            splineEntityLocation.Position = worldPosition;
            if (math.lengthsq(worldTangent) < float.Epsilon) return;
            var forward = math.normalize(worldTangent);
            var up = math.normalize(worldUpVector);
            splineEntityLocation.LookRotationSafe = quaternion.LookRotationSafe(forward, up);
        }
    }
}