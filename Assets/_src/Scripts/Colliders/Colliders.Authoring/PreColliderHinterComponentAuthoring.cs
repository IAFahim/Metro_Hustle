using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class PreColliderHinterComponentAuthoring : MonoBehaviour
    {
        public half forward = new(-2);
        public half radius = new(1);

        private class PreColliderHinterComponentBaker : Baker<PreColliderHinterComponentAuthoring>
        {
            public override void Bake(PreColliderHinterComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PreHitColliderComponent
                {
                    Forward = authoring.forward,
                    RadiusSq = (half)math.pow(authoring.radius, 2)
                });
            }
        }
    }
}