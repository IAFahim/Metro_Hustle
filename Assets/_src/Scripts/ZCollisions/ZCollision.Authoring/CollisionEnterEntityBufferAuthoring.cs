using _src.Scripts.ZCollisions.ZCollision.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.ZCollisions.ZCollision.Authoring
{
    internal class CollisionEnterEntityBufferAuthoring : MonoBehaviour
    {
        private class CollisionEnterBufferBaker : Baker<CollisionEnterEntityBufferAuthoring>
        {
            public override void Bake(CollisionEnterEntityBufferAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddBuffer<CollisionEnterEntityBuffer>(entity);
            }
        }
    }
}