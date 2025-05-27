using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public struct TargetBody : IComponentData
    {
        public half Leg;
        public half ForwardTip;
    }
}