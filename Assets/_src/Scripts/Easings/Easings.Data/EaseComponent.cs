using Unity.Entities;

namespace _src.Scripts.Easings.Runtime.Datas
{
    public struct EaseComponent : IComponentData, IEnableableComponent
    {
        public float Duration;
        public float Elapsed;
        public Ease Ease;
    }
}