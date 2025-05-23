using _src.Scripts.InputControls.InputControls.Data.enums;
using BovineLabs.Core;
using Unity.Entities;

namespace _src.Scripts.InputControls.InputControls.Data
{
    [ChangeFilterTracking]
    public struct DirectionInputEnableActiveComponent : IComponentData
    {
        public DirectionEnableActiveFlag Flag;
    }
}