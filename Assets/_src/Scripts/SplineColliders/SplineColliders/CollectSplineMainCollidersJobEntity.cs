using _src.Scripts.SplineColliders.SplineColliders.Data;
using _src.Scripts.SplineConfigs.SplineConfigs.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.SplineColliders.SplineColliders
{
    [WithAll(typeof(SplineMainColliderTag))]
    public partial struct CollectSplineMainCollidersJobEntity : IJobEntity
    {
        public NativeQueue<ColliderEntityData>.ParallelWriter MainTrackQueue;

        private void Execute(Entity entity, in LocalToWorld localToWorld, in SplineLineComponent splineLineComponent)
        {
            MainTrackQueue.Enqueue(new ColliderEntityData()
            {
                Entity = entity,
                SplineLineComponent = splineLineComponent,
                Position = localToWorld.Position
            });
        }
    }
}