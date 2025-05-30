using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.ZMovements.ZMovements
{
    public struct ZMovementComponent : IComponentData
    {
        public bool IsBackWard;
        public half LeftRightRequest;
        public half Height;
    }
}