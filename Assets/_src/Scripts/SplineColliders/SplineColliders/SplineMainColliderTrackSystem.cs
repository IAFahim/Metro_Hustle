using _src.Scripts.CollisionHints.CollisionHints.Data.Datas;
using _src.Scripts.SplineColliders.SplineColliders.Data;
using _src.Scripts.SplineConfigs.SplineConfigs.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.SplineColliders.SplineColliders
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct SplineMainColliderTrackSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var mainCollidersQueue = new NativeQueue<SplineCollideAbleBuffer>(Allocator.TempJob);
            foreach (
                var (
                    localToWorld,
                    splineLineComponent,
                    entity
                    )
                in SystemAPI.Query<
                        RefRO<LocalToWorld>,
                        RefRO<SplineLineComponent>
                    >()
                    .WithEntityAccess()
                    .WithAll<SplineMainColliderTag>()
            )
            {
                mainCollidersQueue.Enqueue(new()
                {
                    Entity = entity,
                    SplineLineComponent = splineLineComponent.ValueRO,
                    Position = localToWorld.ValueRO.Position
                });
            }

            // var a = collectSplineMainCollidersJobEntity.Schedule(state.Dependency);
            // var preCollisionSystemJobEntity = new SplinePreCollisionSystemJobEntity
            // {
            //     MainColliders = _mainCollidersQueue.ToArray(Allocator.TempJob).AsReadOnly()
            // };
            // var b = preCollisionSystemJobEntity.Schedule(a);
            // var collisionSystemJobEntity = new SplineCollisionSystemJobEntity
            // {
            //     MainColliders = _mainCollidersQueue.ToArray(Allocator.TempJob).AsReadOnly()
            // };
            // collisionSystemJobEntity.ScheduleParallel(b);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}