using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.SplineMovements.SplineMovements.Data
{
    public struct SplineMoveComponent : IComponentData
    {
        public byte CurveIndex;
        public half Speed;
        public half SideOffset;
        public half Distance;
    }
}