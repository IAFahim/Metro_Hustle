using _src.Scripts.BoxLineMovements.BoxLineMovements.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.BoxLineMovements.BoxLineMovements.Authoring
{
    public class BoxLineCollisionRangeComponentAuthoring : MonoBehaviour
    {
        public half height;
        public half radius = new half(4.5f / 3f);

        public class BoxLineCollisionRangeComponentBaker : Baker<BoxLineCollisionRangeComponentAuthoring>
        {
            public override void Bake(BoxLineCollisionRangeComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BoxLineCollisionComponent
                {
                    Height = authoring.height,
                    Radius = authoring.radius
                });
            }
        }
    }
}