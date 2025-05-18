using _src.Scripts.Colliders.Colliders.Data;
using _src.Scripts.CollisionHints.CollisionHints.Data;
using _src.Scripts.CollisionHints.CollisionHints.Data.Datas;
using _src.Scripts.SplineColliders.SplineColliders.Data;
using _src.Scripts.SplineConfigs.SplineConfigs.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.SplineColliders.SplineColliders
{
    [WithAbsent(typeof(SplineMainColliderTag))]
    [BurstCompile]
    public partial struct SplineCollisionSystemJobEntity : IJobEntity
    {
        [ReadOnly] public NativeArray<SplineCollideAbleBuffer>.ReadOnly MainColliders;
        public NativeQueue<CollisionData>.ParallelWriter CollisionDataQueue;

        [BurstCompile]
        private void Execute(
            in SplineLineComponent splineLineComponent,
            in LocalToWorld localToWorld,
            in ColliderUpHeightComponent colliderUpHeightComponent,
            in ColliderRadiusSqComponent colliderRadiusSqComponent,
            in CollisionHitComponent collisionHitComponent
        )
        {
            for (var i = 0; i < MainColliders.Length; i++)
            {
                var main = MainColliders[i];
                if (splineLineComponent.SplineLine != main.SplineLine) continue;
                var colliderOrigin = localToWorld.Position;
                var upOffset = localToWorld.Up * colliderUpHeightComponent.Value;
                if (!main.InSphere(colliderOrigin, upOffset, colliderRadiusSqComponent.RadiusSq)) continue;
                CollisionDataQueue.Enqueue(new ()
                {
                    Entity = main.Entity,
                    CollisionHint = collisionHitComponent.Value
                });
            }
        }
    }
}