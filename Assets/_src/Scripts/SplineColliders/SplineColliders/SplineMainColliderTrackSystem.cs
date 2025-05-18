using _src.Scripts.SplineColliders.SplineColliders.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace _src.Scripts.SplineColliders.SplineColliders
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct SplineMainColliderTrackSystem : ISystem
    {
        private NativeQueue<SplinePointColliderBuffer> _queue;
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _queue = new NativeQueue<SplinePointColliderBuffer>(Allocator.TempJob);
            // var collideAbleList = new NativeList<SplineCollideAbleData>(Allocator.TempJob);
            // foreach (
            //     var (
            //         localToWorld,
            //         splineLineComponent,
            //         entity
            //         )
            //     in SystemAPI.Query<
            //             RefRO<LocalToWorld>,
            //             RefRO<SplineLineComponent>
            //         >()
            //         .WithEntityAccess()
            //         .WithAll<SplineMainColliderTag>()
            // )
            // {
            //     collideAbleList.Add(new()
            //     {
            //         Entity = entity,
            //         SplineLine = splineLineComponent.ValueRO.SplineLine,
            //         Position = localToWorld.ValueRO.Position
            //     });
            // }
            
            // var splineCollideAbleBuffers = SystemAPI.GetSingletonBuffer<SplineCollideAbleData>();
            // splineCollideAbleBuffers.Clear();
            // splineCollideAbleBuffers.AddRange(collideAbleList.AsArray());
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