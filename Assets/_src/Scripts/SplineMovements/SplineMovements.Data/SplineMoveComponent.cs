using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.SplineMovements.SplineMovements.Data
{
    public struct SplineMoveComponent : IComponentData
    {
        public byte CurveIndex;
        public half Speed;
        public half Distance;
        public half Lerp;
    }
}