using _src.Scripts.StatsHelpers.StatsHelpers.Data;
using BovineLabs.Stats.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.ZMovements.ZMovements
{
    [BurstCompile]
    public partial struct ZMoveJobEntity : IJobEntity
    {
        [ReadOnly] public BufferLookup<Stat> StatsBufferLookup;
        public float DeltaTime;

        [BurstCompile]
        private void Execute(Entity entity, ref LocalToWorld localToWorld, ref ZMovementComponent zMovementComponent)
        {
            if (!StatsBufferLookup.TryGetBuffer(entity, out var statsBuffer)) return;
            var statsMap = statsBuffer.AsMap();
            if (statsMap.TryGetValue((byte)EStat.ForwardSpeed, out StatValue forwardSpeed))
            {
                var forward = forwardSpeed.Value * DeltaTime;
                if (zMovementComponent.IsBackWard) forward *= -1;
                localToWorld.Value.c3.z += forward;
            }

            if (statsMap.TryGetValue((byte)EStat.SideWiseSpeed, out StatValue sideSpeed))
            {
                var side = sideSpeed.Value * DeltaTime;
                localToWorld.Value.c3.x += zMovementComponent.LeftRightRequest * side;
            }
        }
    }
}