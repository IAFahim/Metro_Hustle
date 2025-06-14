using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    internal class TrackCollidableBufferSingletonAuthoring : MonoBehaviour
    {
        private class CollisionTrackBufferBaker : Baker<TrackCollidableBufferSingletonAuthoring>
        {
            public override void Bake(TrackCollidableBufferSingletonAuthoring singletonAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddBuffer<TrackCollidableEntityBuffer>(entity);
            }
        }
    }
}