using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    internal class PointColliderComponentAuthoring : MonoBehaviour
    {
        public half forwardPre = new(1);
        public half upOffset = new(.2);

        private class CollisionEnterComponentBaker : Baker<PointColliderComponentAuthoring>
        {
            public override void Bake(PointColliderComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PointContactOffsetComponent
                {
                    ForwardPre = authoring.forwardPre, 
                    UpOffset = authoring.upOffset
                });
            }
        }
    }
}