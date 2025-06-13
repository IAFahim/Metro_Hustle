using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class ColliderRangeComponentAuthoring : MonoBehaviour
    {
        public float range = 1;

        public class ColliderRangeComponentBaker : Baker<ColliderRangeComponentAuthoring>
        {
            public override void Bake(ColliderRangeComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ColliderRangeComponent { Range = authoring.range });
            }
        }
    }
}