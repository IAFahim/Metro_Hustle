using _src.Scripts.Healths.Healths.Data;
using _src.Scripts.StatsHelpers.StatsHelpers.Data;
using BovineLabs.Anchor;
using BovineLabs.Stats.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.Healths.Healths
{
    public partial struct UpdateHealthUISystem : ISystem, ISystemStartStop
    {
        private UIHelper<GameModel, GameModel.Data> _uiHelper;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _uiHelper = new UIHelper<GameModel, GameModel.Data>(
                ref state, ComponentType.ReadOnly<GameScreenTag>());
        }

        public void OnStartRunning(ref SystemState state)
        {
            _uiHelper.Bind();
        }

        public void OnStopRunning(ref SystemState state)
        {
            _uiHelper.Unbind();
        }

        public void OnUpdate(ref SystemState state)
        {
            ref var binding = ref _uiHelper.Binding;

            foreach (var (statsBuffer, ltw) in SystemAPI.Query<DynamicBuffer<Stat>, RefRO<LocalToWorld>>())
            {
                var statsMap = statsBuffer.AsMap();
                var statsKey = new StatKey()
                {
                    Value = (ushort)EStat.MaxHealth
                };
                var currentHealth = statsMap.GetValue(statsKey);
                binding.CurrentHealth = (int)currentHealth;
                binding.HealthNormalized = currentHealth / 2f;
                binding.TotalDistance = (int)ltw.ValueRO.Position.z;
            }
        }
    }
}