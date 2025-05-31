// using _src.Scripts.Healths.Healths.Data;
// using _src.Scripts.StatsHelpers.StatsHelpers.Data;
// using BovineLabs.Anchor;
// using BovineLabs.Anchor.Binding;
// using BovineLabs.Stats.Data;
// using Unity.Burst;
// using Unity.Entities;
//
// namespace _src.Scripts.Healths.Healths
// {
//     // [UpdateInGroup(typeof(ConditionWriteEventsGroup))]
//     // [UpdateBefore(typeof(ConditionEventWriteSystem))]
//     public partial struct UpdateHealthUISystem : ISystem, ISystemStartStop
//     {
//         private UIHelper<HealthViewModel, HealthViewModel.Data> _uiHelper;
//
//
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             _uiHelper = new UIHelper<HealthViewModel, HealthViewModel.Data>(
//                 ref state, ComponentType.ReadOnly<GameScreenTag>());
//         }
//
//         public void OnStartRunning(ref SystemState state)
//         {
//             _uiHelper.Bind();
//         }
//
//         public void OnStopRunning(ref SystemState state)
//         {
//             _uiHelper.Unbind();
//         }
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             ref var binding = ref _uiHelper.Binding;
//
//             foreach (var statsBuffer in SystemAPI.Query<DynamicBuffer<Stat>>())
//             {
//                 var statsMap = statsBuffer.AsMap();
//                 var statsKey = new StatKey()
//                 {
//                     Value = (ushort)EStat.MaxHealth
//                 };
//                 var currentHealth = statsMap.GetValue(statsKey);
//                 binding.CurrentHealth = (int)currentHealth;
//                 binding.HealthNormalized = currentHealth/2f;
//             }
//         }
//     }
// }