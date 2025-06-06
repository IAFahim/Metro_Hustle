using _src.Scripts.Focuses.Focuses.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.Focuses.Focuses
{
    public partial struct FocusMangerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int highestPriority = -1;
            var currentEntity = new Entity();
            foreach (var (focusComponent, entity) in SystemAPI.Query<RefRO<FocusComponent>>().WithEntityAccess())
            {
                if (focusComponent.ValueRO.Priority <= highestPriority) continue;
                highestPriority = focusComponent.ValueRO.Priority;
                currentEntity = entity;
            }

            SystemAPI.GetSingletonRW<FocusCurrentComponent>().ValueRW.Entity = currentEntity;
        }
    }
}