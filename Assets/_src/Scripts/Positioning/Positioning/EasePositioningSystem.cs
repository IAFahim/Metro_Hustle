using _src.Scripts.Colliders.Colliders.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.Positioning.Positioning
{
    public partial struct EasePositioningSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var easePositioningJobEntity = new EasePositioningJobEntity
            {
                LtwLookup = SystemAPI.GetComponentLookup<LocalToWorld>(),
                DeltaTime = SystemAPI.Time.DeltaTime,
                CollidePointLookup = SystemAPI.GetComponentLookup<CollidePointOffsetComponent>(true)
            };
            easePositioningJobEntity.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}