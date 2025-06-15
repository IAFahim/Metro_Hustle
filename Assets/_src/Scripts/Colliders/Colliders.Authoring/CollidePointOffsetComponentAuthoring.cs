using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    internal class CollidePointOffsetComponentAuthoring : MonoBehaviour
    {
        public half forwardPre = new(1);
        public half center = new(0.5f);

        private class CollisionEnterComponentBaker : Baker<CollidePointOffsetComponentAuthoring>
        {
            public override void Bake(CollidePointOffsetComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new CollidePointOffsetComponent
                {
                    ForwardPre = authoring.forwardPre,
                    Center = authoring.center
                });
            }
        }
    }
}