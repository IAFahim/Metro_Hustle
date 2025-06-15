using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Positioning.Positioning.Data
{
    public struct StartPositionRecordComponent : IComponentData, IEnableableComponent
    {
        public float3 Position;
    }
}