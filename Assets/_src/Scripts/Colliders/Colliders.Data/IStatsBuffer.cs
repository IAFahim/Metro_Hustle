using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Data
{
    public struct IStatsBuffer : IBufferElementData
    {
        public Entity Entity;
        public ushort Key;
        public float Value;
    }

    public class StatsBufferAuthoring : MonoBehaviour
    {
        public class StatsBufferBaker : Baker<StatsBufferAuthoring>
        {
            public override void Bake(StatsBufferAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddBuffer<IStatsBuffer>(entity);
            }
        }
    }
}