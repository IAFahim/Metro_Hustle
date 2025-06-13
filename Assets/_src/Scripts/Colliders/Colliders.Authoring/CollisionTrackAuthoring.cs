using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    internal class CollisionTrackAuthoring : MonoBehaviour
    {

        internal class CollisionTrackTagBaker : Baker<CollisionTrackAuthoring>
        {
            public override void Bake(CollisionTrackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new CollisionTrackComponent());
            }
        }
    }
}