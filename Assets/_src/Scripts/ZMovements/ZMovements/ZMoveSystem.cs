using BovineLabs.Stats.Data;
using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.ZMovements.ZMovements
{
    public partial struct ZMoveSystem : ISystem
    {
        private BufferLookup<Stat> _statsBufferLookup;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _statsBufferLookup = state.GetBufferLookup<Stat>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _statsBufferLookup.Update(ref state);
            var zMoveJobEntity = new ZMoveJobEntity
            {
                StatsBufferLookup = _statsBufferLookup,
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            zMoveJobEntity.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}