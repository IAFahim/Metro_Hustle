using _src.Scripts.Easings.Runtime.Datas;
using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.SplineMovements.SplineMovements.Data
{
    public struct SplineSideOffsetComponent : IComponentData
    {
        public half StartOffset;
        public half EndOffset;
        public half EasingT;
        public Ease Ease;
    }
}