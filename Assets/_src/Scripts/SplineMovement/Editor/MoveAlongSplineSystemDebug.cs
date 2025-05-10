#if Aline
using Drawing;
using UnityEngine;
#endif

using _src.Scripts.SplineMovement.Runtime.Datas;
using _src.Scripts.SplineMovement.Runtime.Systems;
using BovineLabs.Core.Groups;
using Unity.Transforms;
using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.SplineMovement.Editor
{
    [BurstCompile]
    
    [UpdateInGroup(typeof(AfterTransformSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    internal partial struct MoveAlongSplineSystemDebug : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
#if Aline
            
            var builder = DrawingManager.GetBuilder();
            foreach (var (
                         localTransform,
                         splineEntityTransformTargetComponentData
                         )
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRO<SplineEntityLocationComponentData>
                     >())

            {
                builder.Arrow(
                    localTransform.ValueRO.Position, splineEntityTransformTargetComponentData.ValueRO.Position
                );
            }

            builder.Dispose();
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}