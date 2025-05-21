using _src.Scripts.InputControls.InputControls.Data.enums;
using Unity.Entities;

namespace _src.Scripts.InputControls.InputControls.Data
{
    public struct DirectionLockComponent : IComponentData
    {
        public DirectionLockFlag Flag;
    }
}