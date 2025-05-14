using Unity.Entities;

namespace _src.Scripts.InputControls.InputControls.Data
{
    public struct DirectionInputThreshold : IComponentData
    {
        public float UpThreshold;
        public float SideThreshold;
        public float DownThreshold;
    }
}