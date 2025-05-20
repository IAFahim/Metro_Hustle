using _src.Scripts.Colliders.Colliders.Data;
using _src.Scripts.Collisions.Collisions.Data.enums;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class ColliderShiftNotifyComponentAuthoring : MonoBehaviour
    {
        public CollisionHint collisionHint;
        public CollisionEffect collisionEffect;

        public class ColliderShiftComponentBaker : Baker<ColliderShiftNotifyComponentAuthoring>
        {
            public override void Bake(ColliderShiftNotifyComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity,
                    new ColliderShiftNotifyComponent
                    {
                        CollisionHint = authoring.collisionHint,
                        CollisionEffect = authoring.collisionEffect
                    }
                );
            }
        }
    }
}