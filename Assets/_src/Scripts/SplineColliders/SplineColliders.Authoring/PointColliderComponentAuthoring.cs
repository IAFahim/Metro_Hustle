using _src.Scripts.Colliders.Colliders.Data.enums;
using _src.Scripts.SplineColliders.SplineColliders.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.SplineColliders.SplineColliders.Authoring
{
    public class PointColliderComponentAuthoring : MonoBehaviour
    {
        public ColliderFlag colliderFlag;

        public class PointColliderComponentBaker : Baker<PointColliderComponentAuthoring>
        {
            public override void Bake(PointColliderComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PointColliderComponent { ColliderFlag = authoring.colliderFlag });
            }
        }
    }
}