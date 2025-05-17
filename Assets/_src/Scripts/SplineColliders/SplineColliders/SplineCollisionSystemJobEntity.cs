using _src.Scripts.Colliders.Colliders.Data;
using _src.Scripts.SplineColliders.SplineColliders.Data;
using _src.Scripts.SplineConfigs.SplineConfigs.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace _src.Scripts.SplineColliders.SplineColliders
{
    [WithAbsent(typeof(SplineMainColliderTag))]
    public partial struct SplineCollisionSystemJobEntity : IJobEntity
    {
        [ReadOnly] public NativeArray<ColliderEntityData>.ReadOnly MainColliders;

        private void Execute(
            in SplineLineComponent splineLineComponent,
            in LocalToWorld localToWorld,
            in ColliderHeightComponent colliderHeightComponent,
            in ColliderRadiusSqComponent colliderRadiusSqComponent
        )
        {
            for (var i = 0; i < MainColliders.Length; i++)
            {
                var main = MainColliders[i];
                if (splineLineComponent.SplineLine != main.SplineLineComponent.SplineLine) continue;
                var upOffset = localToWorld.Up * colliderHeightComponent.Height;
                var spherePosition = localToWorld.Position + upOffset;
                var distance = math.distancesq(spherePosition, main.Position);
                if (distance > colliderRadiusSqComponent.RadiusSq) return;
                Debug.Log("Collided");
            }
        }
    }
}