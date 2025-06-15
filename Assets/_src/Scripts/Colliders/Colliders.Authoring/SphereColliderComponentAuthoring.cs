using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class SphereColliderComponentAuthoring : MonoBehaviour
    {
        public float range = 1f;
        public half destroyRange = new half(0.1);

        public class SphereColliderComponentBaker : Baker<SphereColliderComponentAuthoring>
        {
            public override void Bake(SphereColliderComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SphereColliderComponent
                {
                    LengthSq = authoring.range * authoring.range,
                    DestroySq = (half)(authoring.destroyRange * authoring.destroyRange)
                });
            }
        }
    }
}