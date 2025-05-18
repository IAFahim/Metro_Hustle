using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class ColliderUpHeightComponentAuthoring : MonoBehaviour
    {
        public half height;

        private class ColliderUpHeightComponentBaker : Baker<ColliderUpHeightComponentAuthoring>
        {
            public override void Bake(ColliderUpHeightComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ColliderUpHeightComponent { Value = authoring.height });
            }
        }
    }
}