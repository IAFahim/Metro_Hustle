using _src.Scripts.SplineColliders.SplineColliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.SplineColliders.SplineColliders.Authoring
{
    public class SplineColliderHeightComponentAuthoring : MonoBehaviour
    {
        public half height;

        public class SplineColliderHeightComponentBaker : Baker<SplineColliderHeightComponentAuthoring>
        {
            public override void Bake(SplineColliderHeightComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SplineColliderHeightComponent { Height = authoring.height });
            }
        }
    }
}