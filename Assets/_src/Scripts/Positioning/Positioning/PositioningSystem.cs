using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.Positioning.Positioning
{
    public partial struct PositioningSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var timeDeltaTime = SystemAPI.Time.DeltaTime;
            new PositioningJobComponent
            {
                DeltaTime = timeDeltaTime
            }.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}