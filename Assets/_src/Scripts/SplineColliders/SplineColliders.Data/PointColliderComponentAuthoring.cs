using _src.Scripts.Colliders.Colliders.Data.enums;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace _src.Scripts.SplineColliders.SplineColliders.Data
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