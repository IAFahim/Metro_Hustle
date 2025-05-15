using Unity.Entities;

namespace _src.Scripts.InputControls.InputControls.Data
{
    public struct PlayerInputEnableTag : IComponentData, IEnableableComponent
    {
        public bool Live;
    }
}