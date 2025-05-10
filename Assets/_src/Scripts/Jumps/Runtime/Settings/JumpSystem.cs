using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.Jumps.Runtime.Settings
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct JumpSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            var jumpJob = new JumpJob
            {
                DeltaTime = deltaTime
            };
            jumpJob.Schedule();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}