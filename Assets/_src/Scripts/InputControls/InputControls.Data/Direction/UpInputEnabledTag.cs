using Unity.Entities;

namespace _src.Scripts.InputControls.InputControls.Data.Direction
{
    public struct UpInputEnabledTag : IComponentData, IEnableableComponent
    {
        public bool Active;
    }
}