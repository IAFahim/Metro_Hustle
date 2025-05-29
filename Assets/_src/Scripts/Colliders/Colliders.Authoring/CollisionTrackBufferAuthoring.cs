using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    internal class CollisionTrackBufferAuthoring : MonoBehaviour
    {
        private class CollisionTrackBufferBaker : Baker<CollisionTrackBufferAuthoring>
        {
            public override void Bake(CollisionTrackBufferAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddBuffer<CollisionTrackBuffer>(entity);
            }
        }
    }
}