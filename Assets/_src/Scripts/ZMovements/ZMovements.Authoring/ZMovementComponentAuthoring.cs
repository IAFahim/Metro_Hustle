using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.ZMovements.ZMovements.Authoring
{
    public class ZMovementComponentAuthoring : MonoBehaviour
    {
        public bool isBackWard;
        public half LeftRight;

        private class ZMovementComponentBaker : Baker<ZMovementComponentAuthoring>
        {
            public override void Bake(ZMovementComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ZMovementComponent
                {
                    IsBackWard = authoring.isBackWard,
                    LeftRightRequest = authoring.LeftRight,
                });
            }
        }
    }
}