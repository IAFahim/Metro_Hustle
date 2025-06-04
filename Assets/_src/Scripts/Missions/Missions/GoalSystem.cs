using _src.Scripts.Missions.Missions.Data;
using BovineLabs.Stats.Data;
using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.Missions.Missions
{
    public partial struct GoalSystem : ISystem
    {
        private BufferLookup<Intrinsic> _intrinsicLookup;
        private BufferLookup<GoalBuffer> _goalLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _intrinsicLookup = state.GetBufferLookup<Intrinsic>(true);
            _goalLookup = state.GetBufferLookup<GoalBuffer>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _intrinsicLookup.Update(ref state);
            _goalLookup.Update(ref state);
            new GoalJobEntity()
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                IntrinsicLookup = _intrinsicLookup,
                GoalLookup = _goalLookup
            }.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}