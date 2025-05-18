using _src.Scripts.SplineColliders.SplineColliders.Data;
using _src.Scripts.SplineColliders.SplineColliders.Jobs;
using _src.Scripts.SplineConfigs.SplineConfigs.Data;
using BovineLabs.Reaction.Conditions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.SplineColliders.SplineColliders
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct SplineMainColliderTrackSystem : ISystem
    {
        public ConditionEventWriter.Lookup lookup;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            lookup.Create(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var collideAbleList = new NativeList<SplineCollideAbleBuffer>(Allocator.TempJob);
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
                collideAbleList.Add(new()
                {
                    Entity = entity,
                    SplineLine = splineLineComponent.ValueRO.SplineLine,
                    Position = localToWorld.ValueRO.Position
                });
            }
            
            lookup.Update(ref state);
            new TestCondition()
            {
                Lookup = lookup
            }.Schedule();

            var splineCollideAbleBuffers = SystemAPI.GetSingletonBuffer<SplineCollideAbleBuffer>();
            splineCollideAbleBuffers.Clear();
            splineCollideAbleBuffers.AddRange(collideAbleList.AsArray());
            // new TestBuffer()
            // {
            //     db = splineCollideAbleBuffers.
            // }.ScheduleParallel();

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