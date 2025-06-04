using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.RoadMovements.RoadMovements
{
    public partial struct RoadLeftRightSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new RoadLeftRightJobEntity()
            {
                Road = SystemAPI.GetSingleton<RoadComponent>()
            }.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}