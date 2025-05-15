using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.InputControls.InputControls.Data
{
    public struct TouchInputThresholdSingleton : IComponentData
    {
        public half SideMinimum;
    }
}