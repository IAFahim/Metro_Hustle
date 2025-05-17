using _src.Scripts.SplineColliders.SplineColliders.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace _src.Scripts.SplineColliders.SplineColliders
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct SplineMainColliderTrackSystem : ISystem
    {
        private NativeQueue<ColliderEntityData> _mainCollidersQueue;
        private NativeArray<ColliderEntityData>.ReadOnly _mainColliderArray;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (_mainCollidersQueue.IsCreated)
            {
                _mainColliderArray = _mainCollidersQueue.ToArray(Allocator.TempJob).AsReadOnly();
                var splineCollisionSystemJobEntity = new SplineCollisionSystemJobEntity
                {
                    MainColliders = _mainColliderArray
                };
                splineCollisionSystemJobEntity.ScheduleParallel();
            }

            _mainCollidersQueue = new(Allocator.TempJob);
            var collectSplineMainCollidersJobEntity = new CollectSplineMainCollidersJobEntity
            {
                MainTrackQueue = _mainCollidersQueue.AsParallelWriter()
            };
            collectSplineMainCollidersJobEntity.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}