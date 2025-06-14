using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Positioning.Positioning.Data
{
    public struct LeftRightComponent : IComponentData
    {
        public half Speed;
        public sbyte Direction;
        public half Current;
        public half Target;
    }
}