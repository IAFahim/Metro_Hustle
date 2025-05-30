using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Positioning.Positioning.Data
{
    public struct LeftRightComponent : IComponentData
    {
        public half Offset;
    }
}