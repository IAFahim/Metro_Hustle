using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.SetTransformUsageFlags.SetTransformUsageFlags.Authoring
{
    public class TransformUsageFlagsSetterAuthoring : MonoBehaviour
    {
        public TransformUsageFlags transformUsageFlags;

        public class TransformUsageFlagsSetterBaker : Baker<TransformUsageFlagsSetterAuthoring>
        {
            public override void Bake(TransformUsageFlagsSetterAuthoring authoring)
            {
                GetEntity(authoring.transformUsageFlags);
            }
        }
    }
}