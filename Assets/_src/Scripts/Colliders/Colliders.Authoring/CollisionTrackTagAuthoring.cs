using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Data
{
    internal class CollisionTrackTagAuthoring : MonoBehaviour
    {
        internal class CollisionTrackTagBaker : Baker<CollisionTrackTagAuthoring>
        {
            public override void Bake(CollisionTrackTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<CollisionTrackTag>(entity);
            }
        }
    }
}