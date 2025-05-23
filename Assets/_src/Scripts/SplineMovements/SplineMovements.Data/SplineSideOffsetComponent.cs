using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.SplineMovements.SplineMovements.Data
{
    public struct SplineSideOffsetComponent : IComponentData
    {
        public half CurrentOffset;
        public half EndOffset;
        public half Speed;
        public bool Moving;
    }
}