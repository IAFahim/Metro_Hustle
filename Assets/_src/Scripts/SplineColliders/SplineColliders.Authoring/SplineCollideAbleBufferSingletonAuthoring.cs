using _src.Scripts.SplineColliders.SplineColliders.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.SplineColliders.SplineColliders.Authoring
{
    public class SplineCollideAbleBufferSingletonAuthoring : MonoBehaviour
    {
        public class SplineCollideAbleEntityDataBaker : Baker<SplineCollideAbleBufferSingletonAuthoring>
        {
            public override void Bake(SplineCollideAbleBufferSingletonAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddBuffer<SplineCollideAbleBuffer>(entity);
            }
        }
    }
}