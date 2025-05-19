using _src.Scripts.Colliders.Colliders.Data;
using _src.Scripts.SplineColliders.SplineColliders.Data;
using _src.Scripts.SplineConfigs.SplineConfigs.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.SplineColliders.SplineColliders.Jobs
{
    [BurstCompile]
    public partial struct SplinePreCollisionSystemJobEntity : IJobEntity
    {
        public NativeArray<SplinePointColliderBuffer>.ReadOnly PointColliders;
        public ComponentLookup<ColliderShiftNotifyComponent> ColliderShiftNotifyComponentLookup;

        [BurstCompile]
        private void Execute(
            in SplineLineComponent splineLineComponent,
            in LocalToWorld localToWorld,
            in ColliderPreHitComponent preHitColliderComponent,
            in ColliderFlagComponent colliderFlagComponent,
            in ColliderHeightComponent colliderHeightComponent,
            in ColliderRadiusSqComponent colliderRadiusSqComponent
        )
        {
            for (var i = 0; i < PointColliders.Length; i++)
            {
                var main = PointColliders[i];
                if (splineLineComponent.Value != main.SplineLine) continue;
                var colliderOrigin = localToWorld.Position;
                var backOffset = localToWorld.Forward * preHitColliderComponent.Forward;
                if (!main.InSphere(colliderOrigin, backOffset, preHitColliderComponent.RadiusSq)) continue;
            }
        }
    }
}