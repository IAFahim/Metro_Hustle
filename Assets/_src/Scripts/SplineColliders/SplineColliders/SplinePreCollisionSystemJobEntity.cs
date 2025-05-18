using _src.Scripts.Colliders.Colliders.Data;
using _src.Scripts.CollisionHints.CollisionHints.Data;
using _src.Scripts.CollisionHints.CollisionHints.Data.Datas;
using _src.Scripts.SplineColliders.SplineColliders.Data;
using _src.Scripts.SplineConfigs.SplineConfigs.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace _src.Scripts.SplineColliders.SplineColliders
{
    [WithAbsent(typeof(SplineMainColliderTag))]
    [BurstCompile]
    public partial struct SplinePreCollisionSystemJobEntity : IJobEntity
    {
        [ReadOnly] public NativeArray<SplineCollideAbleBuffer>.ReadOnly MainColliders;
        public NativeQueue<PreCollisionData>.ParallelWriter PreCollisionDataQueue;

        [BurstCompile]
        private void Execute(
            in SplineLineComponent splineLineComponent,
            in LocalToWorld localToWorld,
            in PreHitColliderComponent preHitColliderComponent,
            in PreCollisionHintComponent preCollisionHintComponent
        )
        {
            for (var i = 0; i < MainColliders.Length; i++)
            {
                var main = MainColliders[i];
                if (splineLineComponent.SplineLine != main.SplineLineComponent.SplineLine) continue;
                var colliderOrigin = localToWorld.Position;
                var backOffset = localToWorld.Forward * preHitColliderComponent.Forward;
                if (!main.InSphere(colliderOrigin, backOffset, preHitColliderComponent.RadiusSq)) continue;
                PreCollisionDataQueue.Enqueue(new()
                {
                    Entity = main.Entity,
                    PreCollisionHint = preCollisionHintComponent.Value
                });
            }
        }
    }
}