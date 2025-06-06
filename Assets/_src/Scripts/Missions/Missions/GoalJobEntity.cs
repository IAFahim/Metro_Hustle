using _src.Scripts.Missions.Missions.Data;
using BovineLabs.Stats.Data;
using Unity.Entities;

namespace _src.Scripts.Missions.Missions
{
    public partial struct GoalJobEntity : IJobEntity
    {
        private void Execute(
            ref DynamicBuffer<GoalBuffer> goals,
            in DynamicBuffer<Intrinsic> intrinsicBuffer
        )
        {
            var intrinsic = intrinsicBuffer.AsMap();
            for (var i = 0; i < goals.Length; i++)
            {
                var goal = goals[i];
                if (goal.isComplete) continue;

                var intrinsicKey = new IntrinsicKey(value: (ushort)goal.intrinsic);
                if (!intrinsic.TryGetValue(intrinsicKey, out var count) || !goal.TryComplete(count)) continue;
                
                goal.isComplete = true;
                goals[i] = goal;
            }
        }
    }
}