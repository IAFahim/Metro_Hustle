using _src.Scripts.Healths.Healths.Data;
using BovineLabs.Anchor;
using BovineLabs.Stats.Data;
using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.Healths.Healths
{
    [UpdateInGroup(typeof(UISystemGroup))]
    public partial struct UpdateHealthUISystem : ISystem, ISystemStartStop
    {
        private UIHelper<HealthViewModel, HealthViewModel.Data> _uiHelper;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _uiHelper = new UIHelper<HealthViewModel, HealthViewModel.Data>(
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

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ref var binding = ref _uiHelper.Binding;
            binding.MaxHealth = 100;

            foreach (var intrinsicsBuffer in SystemAPI.Query<DynamicBuffer<Intrinsic>>()
                         .WithAny<IntrinsicConditionDirty>())
            {
                var intrinsicMap = intrinsicsBuffer.AsMap();
                var healthKey = new IntrinsicKey { Value = 0 };
                var health = intrinsicMap.GetValue(healthKey);
                binding.CurrentHealth = health;
            }
        }
    }
}