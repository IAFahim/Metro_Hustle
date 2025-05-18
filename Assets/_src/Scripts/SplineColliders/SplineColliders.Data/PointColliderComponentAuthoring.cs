using _src.Scripts.CollisionHints.CollisionHints.Data.enums;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.SplineColliders.SplineColliders.Data
{
    public class PointColliderComponentAuthoring : MonoBehaviour
    {
        public CollisionFlag collisionFlag;

        public class PointColliderComponentBaker : Baker<PointColliderComponentAuthoring>
        {
            public override void Bake(PointColliderComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PointColliderComponent { CollisionFlag = authoring.collisionFlag });
            }
        }
    }
}