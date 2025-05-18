using _src.Scripts.CollisionHints.CollisionHints.Data;
using _src.Scripts.CollisionHints.CollisionHints.Data.enums;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.CollisionHints.CollisionHints.Authoring
{
    public class PreCollisionHintComponentAuthoring : MonoBehaviour
    {
        public PreCollisionHint preCollisionHint;

        private class ObstacleCollisionHintComponentBaker : Baker<PreCollisionHintComponentAuthoring>
        {
            public override void Bake(PreCollisionHintComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PreCollisionHintComponent
                {
                    Value = authoring.preCollisionHint
                });
            }
        }
    }
}