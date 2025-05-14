using Unity.Entities;

namespace _src.Scripts.InputControls.InputControls.Data.Direction
{
    public struct RightInputEnabledTag : IComponentData, IEnableableComponent
    {
        public bool Live;
    }
}