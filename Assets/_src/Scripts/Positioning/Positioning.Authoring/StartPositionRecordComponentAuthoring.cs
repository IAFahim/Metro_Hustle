using _src.Scripts.Positioning.Positioning.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Positioning.Positioning.Authoring
{
    internal class StartPositionRecordComponentAuthoring : MonoBehaviour
    {
        public float3 position;
        public bool recorded;

        private class StartPositionRecordComponentBaker : Baker<StartPositionRecordComponentAuthoring>
        {
            public override void Bake(StartPositionRecordComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new StartPositionRecordComponent { Position = authoring.position });
                SetComponentEnabled<StartPositionRecordComponent>(entity, authoring.recorded);
            }
        }
    }
}