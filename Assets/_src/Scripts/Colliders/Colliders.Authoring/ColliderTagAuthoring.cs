using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    internal class ColliderTagAuthoring : MonoBehaviour
    {
        private class ColliderTagBaker : Baker<ColliderTagAuthoring>
        {
            public override void Bake(ColliderTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<ColliderTag>(entity);
            }
        }
    }
}