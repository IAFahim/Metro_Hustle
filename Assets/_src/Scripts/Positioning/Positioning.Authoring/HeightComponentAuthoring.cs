using _src.Scripts.Positioning.Positioning.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Positioning.Positioning.Authoring
{
    [DisallowMultipleComponent]
    public class HeightComponentAuthoring : MonoBehaviour
    {
        public half height;

        public class HeightComponentBaker : Baker<HeightComponentAuthoring>
        {
            public override void Bake(HeightComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new HeightComponent { Value = authoring.height });
            }
        }
    }
}