using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class ColliderHeightComponentAuthoring : MonoBehaviour
    {
        public half up = new(0);
        public half radius = new(.5f);
        public class ColliderHeightComponentBaker : Baker<ColliderHeightComponentAuthoring>
        {
            public override void Bake(ColliderHeightComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<ColliderHeightComponent>(entity);
                AddComponent(entity, new ColliderHeightComponent
                {
                    Up = authoring.up
                });

                AddComponent(entity, new ColliderRadiusSqComponent
                {
                    RadiusSq = (half)math.pow(authoring.radius, 2)
                });
            }
        }
    }
}