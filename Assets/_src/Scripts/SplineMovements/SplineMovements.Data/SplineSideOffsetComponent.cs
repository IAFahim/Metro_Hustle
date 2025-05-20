using _src.Scripts.Easings.Runtime.Datas;
using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.SplineMovements.SplineMovements.Data
{
    public struct SplineSideOffsetComponent : IComponentData
    {
        public Ease SideEase;
        public half SideOffset;
        public half TargetSideOffset;
        public half SideT;
    }
}