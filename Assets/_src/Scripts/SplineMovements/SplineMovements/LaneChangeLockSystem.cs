using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.SplineMovements.SplineMovements
{
    public partial struct LaneChangeLockSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            new LaneChangeLockJobEntity
            {
                TimeDelta = SystemAPI.Time.DeltaTime
            }.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}