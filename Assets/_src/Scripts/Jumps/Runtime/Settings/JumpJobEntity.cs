using _src.Scripts.Easings.Runtime.Datas;
using _src.Scripts.Jumps.Runtime.Datas;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.Jumps.Runtime.Settings
{
    public partial struct JumpSystem
    {
        [BurstCompile]
        private partial struct JumpJob : IJobEntity
        {
            public float DeltaTime;

            void Execute(ref JumpComponentData jump, ref LocalTransform transform)
            {
                jump.ElapsedTime += DeltaTime;
                if (IsProcessingRiseStage(ref jump, out float riseEndDuration)) return;
                if (IsProcessingAirStage(ref jump, riseEndDuration, out float endAirEndDuration)) return;
                if (IsProcessingFallStage(ref jump, endAirEndDuration)) return;
            }

            [BurstCompile]
            private static bool IsProcessingFallStage(ref JumpComponentData jump, float endAirTime)
            {
                var fallTime = jump.FallDuration * jump.FallDurationMultiplier;
                var endFallTime = endAirTime + fallTime;
                if (jump.ElapsedTime > endFallTime) return false;
                float t = (endFallTime - jump.ElapsedTime) / fallTime;
                jump.CurrentHeight = jump.FallEase.Evaluate(t) * jump.MaxHeight;
                return true;
            }

            [BurstCompile]
            private static bool IsProcessingAirStage(ref JumpComponentData jump, float riseDuration,
                out float endAirTime)
            {
                endAirTime = riseDuration + jump.AirTime;
                return jump.ElapsedTime < endAirTime;
            }

            [BurstCompile]
            private static bool IsProcessingRiseStage(ref JumpComponentData jump, out float riseEndDuration)
            {
                riseEndDuration = jump.RiseDuration * jump.RiseDurationMultiplier;
                if (jump.ElapsedTime > riseEndDuration) return false;
                float t = jump.ElapsedTime / riseEndDuration;
                jump.CurrentHeight = jump.RiseEase.Evaluate(t) * jump.MaxHeight;
                return true;
            }
        }
    }
}