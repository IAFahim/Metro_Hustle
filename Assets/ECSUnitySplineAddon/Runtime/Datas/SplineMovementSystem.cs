using ECSSplines.Runtime;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS_Spline.Runtime.Datas
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial struct SplineMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SplineContainerBlobComponent>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            var splineContainerBlobRef = SystemAPI.GetSingleton<SplineContainerBlobComponent>().Value;
            foreach (var (transform, splinePos)
                     in SystemAPI.Query<RefRW<LocalTransform>, RefRW<SplinePositionData>>())
            {
                if (!splineContainerBlobRef.IsCreated)
                    continue;

                ref var blob = ref splineContainerBlobRef.Value;
                int splineIndex = splinePos.ValueRO.TargetSplineIndex;

                if (splineIndex < 0 || splineIndex >= blob.SplineCount) continue;

                ref readonly var meta = ref blob.GetSplineMetadata(splineIndex);

                float currentNormalizedT = splinePos.ValueRO.NormalizedTime;
                float speed = splinePos.ValueRO.Speed;
                float length = meta.Length;

                if (length > 0f)
                {
                    float distanceDelta = speed * deltaTime;
                    float tDelta = distanceDelta / length;

                    currentNormalizedT += tDelta;

                    if (meta.Closed)
                    {
                        currentNormalizedT = currentNormalizedT % 1.0f;
                        if (currentNormalizedT < 0) currentNormalizedT += 1.0f;
                    }
                    else
                    {
                        currentNormalizedT = math.clamp(currentNormalizedT, 0f, 1f);
                    }
                }
                else currentNormalizedT = 0f;


                splinePos.ValueRW.NormalizedTime = currentNormalizedT;

                blob.Evaluate(splineIndex, currentNormalizedT, out float3 splinePosition, out float3 splineTangent,
                    out float3 splineUp);

                float3 forwardDir = math.normalizesafe(splineTangent, new float3(0, 0, 1));
                float3 upDir = math.normalizesafe(splineUp, new float3(0, 1, 0));

                float3 rightDir = math.normalizesafe(math.cross(upDir, forwardDir));
                upDir = math.normalizesafe(math.cross(forwardDir, rightDir));


                float3 offset = splinePos.ValueRO.Offset;
                float3 worldPosition = splinePosition +
                                       rightDir * offset.x +
                                       upDir * offset.y +
                                       forwardDir * offset.z;

                quaternion worldRotation = quaternion.LookRotationSafe(forwardDir, upDir);

                transform.ValueRW.Position = worldPosition;
                transform.ValueRW.Rotation = worldRotation;
            }
        }
    }
}