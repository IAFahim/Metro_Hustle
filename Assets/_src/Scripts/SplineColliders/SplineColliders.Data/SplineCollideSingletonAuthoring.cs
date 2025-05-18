using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.SplineColliders.SplineColliders.Data
{
    public class SplineCollideSingletonAuthoring : MonoBehaviour
    {
        public class SplineCollideSingletonBaker : Baker<SplineCollideSingletonAuthoring>
        {
            public override void Bake(SplineCollideSingletonAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<SplineCollideSingleton>(entity);
            }
        }
    }
}