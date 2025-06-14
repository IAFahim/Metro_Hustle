using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class SphereColliderComponentAuthoring : MonoBehaviour
    {
        public half range = new(1);

        public class SphereColliderComponentBaker : Baker<SphereColliderComponentAuthoring>
        {
            public override void Bake(SphereColliderComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SphereColliderComponent
                {
                    Range = authoring.range
                });
            }
        }
    }
}