using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.Gaps.Gaps.Data
{
    public struct GapZComponent : IComponentData
    {
        public half Forward;
        public half Backward;
    }
}
