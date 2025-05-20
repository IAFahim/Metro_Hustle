using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.InputConsumers.InputConsumers
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct InputConsumerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}