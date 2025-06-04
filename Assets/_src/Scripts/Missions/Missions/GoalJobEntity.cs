using _src.Scripts.Missions.Missions.Data;
using BovineLabs.Stats.Data;
using Unity.Collections;
using Unity.Entities;

namespace _src.Scripts.Missions.Missions
{
    public partial struct GoalJobEntity : IJobEntity
    {
        public float DeltaTime;
        [ReadOnly] public BufferLookup<Intrinsic> IntrinsicLookup;
        [ReadOnly] public BufferLookup<GoalBuffer> GoalLookup;

        private void Execute(Entity entity, ref GoalCompleteComponent complete)
        {
            if (!IntrinsicLookup.TryGetBuffer(entity, out var dynamicBuffer)) return;
            if (!GoalLookup.TryGetBuffer(entity, out var goals)) return;
            var intrinsic = dynamicBuffer.AsMap();
            for (var i = 0; i < goals.Length; i++)
            {
                var goal = goals[i];
                if ((1 << 0 & complete.CompleteBit) == 1) return;
                var intrinsicKey = new IntrinsicKey(value: (ushort)goal.intrinsic);
                if (intrinsic.TryGetValue(intrinsicKey, out var count))
                {
                    if (!goal.IsComplete(count)) return;
                    var bitFlip = 1 << i;
                    complete.CompleteBit = (byte)(bitFlip | complete.CompleteBit);
                }
            }
        }
    }
}