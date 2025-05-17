using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class ColliderHeightComponentAuthoring : MonoBehaviour
    {
        public half height;

        private class SplineColliderHeightComponentBaker : Baker<ColliderHeightComponentAuthoring>
        {
            public override void Bake(ColliderHeightComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ColliderHeightComponent { Height = authoring.height });
            }
        }
    }
}