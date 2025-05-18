using _src.Scripts.Collisions.Collisions.Data;
using _src.Scripts.Collisions.Collisions.Data.enums;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Collisions.Collisions.Authoring
{
    public class CollisionComponentAuthoring : MonoBehaviour
    {
        public CollisionHint hint;
        public CollisionEffect effect;

        public class CollisionComponentBaker : Baker<CollisionComponentAuthoring>
        {
            public override void Bake(CollisionComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new CollisionComponent { Hint = authoring.hint, Effect = authoring.effect });
            }
        }
    }
}