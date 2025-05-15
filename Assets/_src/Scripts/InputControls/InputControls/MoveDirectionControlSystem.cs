using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.Direction;
using BovineLabs.Core.Input;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.InputControls.InputControls
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct MoveDirectionControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var inputComponentEntity = SystemAPI.GetSingletonEntity<InputCommon>();
            var inputCommon = SystemAPI.GetSingleton<InputCommon>();

            if (inputCommon.InputOverUI) return;
#if UNITY_EDITOR
            if (!inputCommon.CursorInViewPort) return;
#endif


            var inputComponent = SystemAPI.GetComponent<InputComponent>(inputComponentEntity);
            var threshold = SystemAPI.GetComponent<TouchInputThresholdSingleton>(inputComponentEntity);
            var moveDelta = inputComponent.MoveDelta;

            DirectionFlag determinedRawInputFlag = DirectionFlag.None;
            var absDelta = math.abs(moveDelta);
            var isUpDownDominant = absDelta.y > absDelta.x;
            if (isUpDownDominant)
            {
                if (absDelta.y < threshold.Vertical) return;
                if (moveDelta.y > 0) determinedRawInputFlag |= DirectionFlag.IsUp;
                else if (moveDelta.y < 0) determinedRawInputFlag |= DirectionFlag.IsDown;
            }
            else
            {
                if (absDelta.x < threshold.Horizontal) return;
                if (moveDelta.x > 0) determinedRawInputFlag |= DirectionFlag.IsRight;
                else if (moveDelta.x < 0) determinedRawInputFlag |= DirectionFlag.IsLeft;
            }

            MoveDirectionIJobEntity moveDirectionIJobEntity = new MoveDirectionIJobEntity()
            {
                RawInputCommand = determinedRawInputFlag
            };

            // Schedule the job with dependency management
            state.Dependency = moveDirectionIJobEntity.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}