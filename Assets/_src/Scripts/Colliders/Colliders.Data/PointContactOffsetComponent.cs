using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Colliders.Colliders.Data
{
    public struct PointContactOffsetComponent : IComponentData
    {
        public half ForwardPre;
        public half UpOffset;
    }
}