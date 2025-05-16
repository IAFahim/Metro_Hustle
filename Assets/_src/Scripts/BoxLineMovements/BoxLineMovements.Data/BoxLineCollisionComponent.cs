using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.BoxLineMovements.BoxLineMovements.Data
{
    public struct BoxLineCollisionComponent : IComponentData
    {
        public half Height;
        public half Radius;
    }
}