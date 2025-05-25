using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public struct TargetTrack : IComponentData
    {
        public half Leg;
        public half ForwardTip;
    }
}