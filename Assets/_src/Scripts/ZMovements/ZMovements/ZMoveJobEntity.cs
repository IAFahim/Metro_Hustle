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
        private void Execute(Entity entity, ref LocalToWorld localToWorld)
        {
            if (!StatsBufferLookup.TryGetBuffer(entity, out var statsBuffer)) return;
            var statsMap = statsBuffer.AsMap();
            var key = (byte)EStat.MoveSpeed;

            if (statsMap.TryGetValue(key, out StatValue statValue))
            {
                var forward = statValue.Value * DeltaTime;
                localToWorld.Value.c3.z += forward;
            }
        }
    }
}