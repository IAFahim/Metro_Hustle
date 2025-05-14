using Unity.Entities;

namespace _src.Scripts.InputControls.InputControls.Data.Direction
{
    public struct DownInputEnabledTag : IComponentData, IEnableableComponent
    {
        public bool Active;
    }
}