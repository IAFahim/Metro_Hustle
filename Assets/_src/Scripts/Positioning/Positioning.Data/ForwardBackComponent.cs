using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Positioning.Positioning.Data
{
    public struct ForwardBackComponent : IComponentData
    {
        public half Offset;
    }
}