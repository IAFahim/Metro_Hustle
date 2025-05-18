using _src.Scripts.CollisionHints.CollisionHints.Data;
using _src.Scripts.CollisionHints.CollisionHints.Data.enums;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.CollisionHints.CollisionHints.Authoring
{
    public class CollisionHintComponentAuthoring : MonoBehaviour
    {
        public CollisionHint collisionHintComponent;

        public class CollisionHintComponentBaker : Baker<CollisionHintComponentAuthoring>
        {
            public override void Bake(CollisionHintComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CollisionHitComponent
                {
                    Value = authoring.collisionHintComponent
                });
            }
        }
    }
}