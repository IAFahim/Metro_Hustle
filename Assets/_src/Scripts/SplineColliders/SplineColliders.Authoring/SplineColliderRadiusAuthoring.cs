using _src.Scripts.SplineColliders.SplineColliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.SplineColliders.SplineColliders.Authoring
{
    public class SplineColliderRadiusAuthoring : MonoBehaviour
    {
        public half radius = new(4.5f / 3f);

        public class SplineColliderRadiusBaker : Baker<SplineColliderRadiusAuthoring>
        {
            public override void Bake(SplineColliderRadiusAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SplineColliderRadiusComponent
                {
                    Radius = authoring.radius
                });
            }
        }
    }
}