using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Easings.Easings.Data
{
    public struct EaseComponent : IComponentData, IEnableableComponent
    {
        public half Duration;
        public half Elapsed;
        public Ease Ease;
    }
}