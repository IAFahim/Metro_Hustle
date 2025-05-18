using _src.Scripts.Colliders.Colliders.Data;
using _src.Scripts.Colliders.Colliders.Data.enums;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class ColliderFlagComponentAuthoring : MonoBehaviour
    {
        public ColliderFlag colliderFlag;

        public class ColliderFlagComponentBaker : Baker<ColliderFlagComponentAuthoring>
        {
            public override void Bake(ColliderFlagComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ColliderFlagComponent { ColliderFlag = authoring.colliderFlag });
            }
        }
    }
}