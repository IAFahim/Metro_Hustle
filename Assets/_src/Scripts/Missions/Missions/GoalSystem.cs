using _src.Scripts.Focuses.Focuses.Data;
using _src.Scripts.Healths.Healths.Data;
using _src.Scripts.Missions.Missions.Data;
using BovineLabs.Anchor;
using BovineLabs.Core;
using BovineLabs.Core.SubScenes;
using BovineLabs.Sample.UI.Views.Menu;
using BovineLabs.Stats.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace _src.Scripts.Missions.Missions
{
    public partial struct GoalSystem : ISystem, ISystemStartStop
    {
        private float _time;
        private NativeList<Goal> _goals;

        private UIHelper<GameModel, GameModel.Data> _uiHelper;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _uiHelper = new UIHelper<GameModel, GameModel.Data>(
                ref state, ComponentType.ReadOnly<GameScreenTag>());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var focusSingletonComponent = SystemAPI.GetSingleton<FocusSingletonComponent>();
            var mainEntity = focusSingletonComponent.Entity;
            _time -= SystemAPI.Time.DeltaTime;
            if (_time < 0)
            {
                ref var binding = ref _uiHelper.Binding;
                binding.ExitGame = true;
                return;
            }

            var intrinsics = SystemAPI.GetBuffer<Intrinsic>(mainEntity).AsMap();
            for (var i = _goals.Length - 1; i >= 0; i--)
            {
                var goal = _goals[i];
                var value = intrinsics[new IntrinsicKey { Value = (ushort)goal.intrinsic }];
                if (goal.TryComplete(value, out var progress))
                {
                    _goals.RemoveAt(i);
                }
            }
        }

        public void OnStartRunning(ref SystemState state)
        {
            var (mission, missionNumber) = HomeView.MissionsSettings.GetCurrent();
            this._time = mission.time;
            _goals = new(Allocator.Persistent);
            foreach (var objective in mission.objectives)
            {
                if (!objective.goal.isComplete) _goals.Add(objective.goal);
            }

            _uiHelper.Bind();
        }

        public void OnStopRunning(ref SystemState state)
        {
            _goals.Dispose();
            _uiHelper.Unbind();
            // HomeView.ExitGameWorld();
        }
    }
}