using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.ZMovements.ZMovements.Data
{
    public class ZMovementComponentAuthoring : MonoBehaviour
    {
        public bool enable;
        private class ZMovementComponentBaker : Baker<ZMovementComponentAuthoring>
        {
            public override void Bake(ZMovementComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<ZMovementComponent>(entity);
                SetComponentEnabled<ZMovementComponent>(entity, authoring.enable);
            }
        }
    }
}