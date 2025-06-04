using _src.Scripts.Missions.Missions.Data;
using BovineLabs.Stats.Data;
using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.Missions.Missions
{
    public partial struct GoalSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var isActive = MissionsSettings.IsActive;
            new GoalJobEntity()
            {
            }.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}