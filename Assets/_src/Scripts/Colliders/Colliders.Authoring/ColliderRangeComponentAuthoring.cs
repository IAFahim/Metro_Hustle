using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class ColliderRangeComponentAuthoring : MonoBehaviour
    {
        public half range = new(1);

        public class SphereColliderComponentBaker : Baker<ColliderRangeComponentAuthoring>
        {
            public override void Bake(ColliderRangeComponentAuthoring authoring)
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