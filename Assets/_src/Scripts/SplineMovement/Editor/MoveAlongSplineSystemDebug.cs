#if ALINE
using Drawing;
#endif

using _src.Scripts.SplineMovement.Runtime.Datas;
using BovineLabs.Core.Groups;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

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
#if ALINE
            
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

            builder.DisposeAfter(state.Dependency);
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}