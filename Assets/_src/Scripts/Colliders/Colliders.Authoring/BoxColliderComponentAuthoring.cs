using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class BoxColliderComponentAuthoring : MonoBehaviour
    {
        public half3 halfExtents = new(1);
        public class BoxColliderComponentBaker : Baker<BoxColliderComponentAuthoring>
        {
            public override void Bake(BoxColliderComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new BoxColliderComponent
                {
                    HalfExtents = authoring.halfExtents
                });
            }
        }
    }
}