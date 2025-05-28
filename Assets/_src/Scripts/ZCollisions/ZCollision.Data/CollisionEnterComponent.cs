using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.ZCollisions.ZCollision.Data
{
    public struct CollisionEnterComponent : IComponentData
    {
        public half ForwardPre;
        public half UpOffset;
    }
}