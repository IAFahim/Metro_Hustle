using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class ColliderRadiusComponentAuthoring : MonoBehaviour
    {
        public half radius = new(math.pow(4.5f / 3f, 2));

        private class SplineColliderRadiusBaker : Baker<ColliderRadiusComponentAuthoring>
        {
            public override void Bake(ColliderRadiusComponentAuthoring componentAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ColliderRadiusSqComponent
                {
                    RadiusSq = new half(math.pow(componentAuthoring.radius, 2))
                });
            }
        }
    }
}