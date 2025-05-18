using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class PreHitSphereComponentAuthoring : MonoBehaviour
    {
        public half forward = new(2);
        public half radius = new(1);

        private class SphereColliderWithPreHitComponentBaker : Baker<PreHitSphereComponentAuthoring>
        {
            public override void Bake(PreHitSphereComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new ColliderPreHitComponent
                {
                    Forward = authoring.forward,
                    RadiusSq = (half)math.pow(authoring.radius, 2)
                });
            }
        }
    }
}