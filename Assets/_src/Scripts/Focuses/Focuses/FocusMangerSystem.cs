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
            LocalToWorld highestPriorityLocalTransform = new LocalToWorld();
            foreach (
                var (
                    focusComponent,
                    localTransform
                    )
                in SystemAPI.Query<
                    RefRO<FocusComponent>,
                    RefRO<LocalToWorld>
                >())
            {
                if (focusComponent.ValueRO.Priority > highestPriority)
                {
                    highestPriority = focusComponent.ValueRO.Priority;
                    highestPriorityLocalTransform = localTransform.ValueRO;
                }
            }

            SystemAPI.GetSingletonRW<FocusManagerCurrentInfoComponent>().ValueRW = new FocusManagerCurrentInfoComponent
            {
                Position = highestPriorityLocalTransform.Position,
                Rotation = highestPriorityLocalTransform.Rotation
            };
        }
    }
}