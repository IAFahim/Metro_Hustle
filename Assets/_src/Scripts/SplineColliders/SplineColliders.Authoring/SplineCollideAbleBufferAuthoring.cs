using _src.Scripts.SplineColliders.SplineColliders.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.SplineColliders.SplineColliders.Authoring
{
    public class SplineCollideAbleBufferAuthoring : MonoBehaviour
    {
        private class SplineCollideAbleBufferBaker : Baker<SplineCollideAbleBufferAuthoring>
        {
            public override void Bake(SplineCollideAbleBufferAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddBuffer<SplinePointColliderBuffer>(entity);
            }
        }
    }
}