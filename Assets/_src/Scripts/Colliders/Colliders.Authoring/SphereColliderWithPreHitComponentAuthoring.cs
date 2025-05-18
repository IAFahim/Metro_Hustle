using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class SphereColliderWithPreHitComponentAuthoring : MonoBehaviour
    {
        public half up = new(0);
        public half radius = new(.5f);
        public half preHitForward = new(2);
        public half preHitRadius = new(1);

        private class SphereColliderWithPreHitComponentBaker : Baker<SphereColliderWithPreHitComponentAuthoring>
        {
            public override void Bake(SphereColliderWithPreHitComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new ColliderHeightComponent
                {
                    Up = authoring.up
                });

                AddComponent(entity, new ColliderRadiusSqComponent
                {
                    RadiusSq = (half)math.pow(authoring.radius, 2)
                });

                AddComponent(entity, new ColliderPreHitComponent
                {
                    Forward = authoring.preHitForward,
                    RadiusSq = (half)math.pow(authoring.preHitRadius, 2)
                });
            }
        }
    }
}