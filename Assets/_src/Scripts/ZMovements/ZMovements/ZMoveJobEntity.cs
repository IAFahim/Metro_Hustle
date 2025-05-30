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
        private void Execute(Entity entity, ref LocalToWorld localToWorld, ZMovementComponent zMovementComponent)
        {
            if (!StatsBufferLookup.TryGetBuffer(entity, out var statsBuffer)) return;
            var statsMap = statsBuffer.AsMap();
            var key = (byte)EStat.MoveSpeed;
            var forward = 0f;
            if (statsMap.TryGetValue(key, out StatValue statValue))
            {
                forward = statValue.Value * DeltaTime;
            }

            if (zMovementComponent.IsBackWard) forward *= -1;
            localToWorld.Value.c3.z += forward;
            localToWorld.Value.c3.x += zMovementComponent.LeftRightOffset;
        }
    }
}