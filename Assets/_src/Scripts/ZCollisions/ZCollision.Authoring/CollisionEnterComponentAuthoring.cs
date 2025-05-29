using _src.Scripts.ZCollisions.ZCollision.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.ZCollisions.ZCollision.Authoring
{
    public class CollisionEnterComponentAuthoring : MonoBehaviour
    {
        public half forwardPre = new(1);
        public half upOffset = new(.2);

        public class CollisionEnterComponentBaker : Baker<CollisionEnterComponentAuthoring>
        {
            public override void Bake(CollisionEnterComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CollisionEnterComponent
                {
                    ForwardPre = authoring.forwardPre, 
                    UpOffset = authoring.upOffset
                });
            }
        }
    }
}