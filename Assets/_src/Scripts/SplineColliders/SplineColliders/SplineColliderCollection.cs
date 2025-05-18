using _src.Scripts.SplineColliders.SplineColliders.Data;
using _src.Scripts.SplineColliders.SplineColliders.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace _src.Scripts.SplineColliders.SplineColliders
{
    public partial struct SplineColliderCollection : ISystem
    {
        private NativeQueue<SplineCollideAbleBuffer> _queue;
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (_queue.IsCreated)
            {
                var splineCollideAbleBuffers = SystemAPI.GetSingletonBuffer<SplineCollideAbleBuffer>();
                int index = 0;
                while (!_queue.IsEmpty())
                {
                    splineCollideAbleBuffers.Insert(index++,_queue.Dequeue());
                }
            }

            _queue = new(Allocator.TempJob);
            new SplineColliderCollectionJobEntity
            {
                ColliderQueue = _queue.AsParallelWriter()
            }.ScheduleParallel();
        }
    }
}