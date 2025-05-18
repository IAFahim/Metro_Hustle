using _src.Scripts.SplineColliders.SplineColliders.Data;
using _src.Scripts.SplineConfigs.SplineConfigs.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.SplineColliders.SplineColliders.Jobs
{
    [BurstCompile]
    public partial struct SplineColliderCollectionJobEntity : IJobEntity
    {
        public NativeQueue<SplineCollideAbleBuffer>.ParallelWriter ColliderQueue;

        private void Execute(
            Entity entity,
            in LocalToWorld localToWorld,
            in SplineLineComponent splineLineComponent,
            in PointColliderComponent pointColliderComponent
        )
        {
            ColliderQueue.Enqueue(new SplineCollideAbleBuffer()
            {
                Entity = entity,
                SplineLine = splineLineComponent.Value,
                Position = localToWorld.Position,
                CollisionFlag = pointColliderComponent.CollisionFlag
            });
        }
    }
}