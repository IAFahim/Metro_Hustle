using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    internal class TrackEntityCollidableAuthoring : MonoBehaviour
    {
        internal class CollisionTrackTagBaker : Baker<TrackEntityCollidableAuthoring>
        {
            public override void Bake(TrackEntityCollidableAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new TrackCollidableTag());
            }
        }
    }
}