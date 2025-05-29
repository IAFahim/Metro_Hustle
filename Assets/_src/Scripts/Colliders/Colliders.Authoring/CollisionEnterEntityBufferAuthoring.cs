using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    internal class CollisionEnterEntityBufferAuthoring : MonoBehaviour
    {
        private class CollisionEnterBufferBaker : Baker<CollisionEnterEntityBufferAuthoring>
        {
            public override void Bake(CollisionEnterEntityBufferAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddBuffer<CollisionEnterEntityBuffer>(entity);
            }
        }
    }
}