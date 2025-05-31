using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Positioning.Positioning.Data
{
    public struct GravityEnableComponent : IComponentData
    {
        public bool Enable;
        public half Gravity;
        public float GravityMul;
        public half Velocity;
    }
}