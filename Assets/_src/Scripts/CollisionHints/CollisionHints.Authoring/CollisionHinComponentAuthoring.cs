using _src.Scripts.CollisionHints.CollisionHints.Data.enums;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.CollisionHints.CollisionHints.Data
{
    public class CollisionHinComponentAuthoring : MonoBehaviour
    {
        public CollisionHint collisionHintComponent;

        public class CollisionHintComponentBaker : Baker<CollisionHinComponentAuthoring>
        {
            public override void Bake(CollisionHinComponentAuthoring authoring)
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