#if ALINE
using _src.Scripts.Colliders.Colliders.Data;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Editor
{
    public partial struct SphereColliderEditorJobEntity : IJobEntity
    {
        [ReadOnly] public NativeArray<TrackCollidableEntityBuffer>.ReadOnly TrackCollidableEntityBuffer;
        private void Execute(Entity entity, in SphereColliderComponent colliderComponent)
        {
            foreach (var trackCollidableEntityBuffer in TrackCollidableEntityBuffer)
            {
                Debug.Log(entity);
            }
        }

    }
}
#endif