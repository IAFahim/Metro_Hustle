using BovineLabs.Core;
using Unity.Entities;

namespace _src.Scripts.InputControls.InputControls.Data.Direction
{
    [ChangeFilterTracking]
    public struct DirectionInputComponent : IComponentData
    {
        public DirectionFlag DirectionFlag;
    }
}