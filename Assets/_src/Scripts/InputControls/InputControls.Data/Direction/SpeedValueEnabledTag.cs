using Unity.Entities;

namespace _src.Scripts.InputControls.InputControls.Data.Direction
{
    public struct SpeedValueEnabledTag : IComponentData, IEnableableComponent
    {
        public sbyte Level;
    }
}