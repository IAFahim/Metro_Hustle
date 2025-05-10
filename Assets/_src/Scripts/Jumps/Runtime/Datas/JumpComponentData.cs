using _src.Scripts.Easings.Runtime.Datas;
using Unity.Entities;

namespace _src.Scripts.Jumps.Runtime.Datas
{
    public struct JumpComponentData : IComponentData
    {
        public float CurrentHeight;
        public float MaxHeight;
        public float ElapsedTime;
        public Ease RiseEase;
        public float RiseDuration;
        public float RiseDurationMultiplier;
        public float AirTime;
        public Ease FallEase;
        public float FallDuration;
        public float FallDurationMultiplier;
    }
}