using _src.Scripts.SplineColliders.SplineColliders.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.SplineColliders.SplineColliders.Authoring
{
    public class SplineMainColliderTagAuthoring : MonoBehaviour
    {
        public class SplineMainColliderTagBaker : Baker<SplineMainColliderTagAuthoring>
        {
            public override void Bake(SplineMainColliderTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<SplineMainColliderTag>(entity);
            }
        }
    }
}