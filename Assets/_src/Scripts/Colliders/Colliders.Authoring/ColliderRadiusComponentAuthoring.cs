using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class ColliderRadiusComponentAuthoring : MonoBehaviour
    {
        public half radius = new(4.5f / 3f);

        private class SplineColliderRadiusBaker : Baker<ColliderRadiusComponentAuthoring>
        {
            public override void Bake(ColliderRadiusComponentAuthoring componentAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ColliderRadiusComponent
                {
                    Radius = componentAuthoring.radius
                });
            }
        }
    }
}