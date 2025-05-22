using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.enums;
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

            if (!inputCommon.AnyButtonPress)
            {
                new MoveDirectionIJobEntity()
                {
                    RawInputCommand = DirectionEnableActiveFlag.EnableFlagsMask
                }.ScheduleParallel();
                return;
            }

            if (inputCommon.InputOverUI)
            {
                new MoveDirectionIJobEntity()
                {
                    RawInputCommand = DirectionEnableActiveFlag.EnableFlagsMask
                }.ScheduleParallel();
                return;
            }
#if UNITY_EDITOR
            if (!inputCommon.CursorInViewPort)
            {
                new MoveDirectionIJobEntity()
                {
                    RawInputCommand = DirectionEnableActiveFlag.EnableFlagsMask
                }.ScheduleParallel();
                return;
            }
#endif


            var inputComponent = SystemAPI.GetComponent<InputComponent>(inputComponentEntity);
            var threshold = SystemAPI.GetComponent<TouchInputThresholdSingleton>(inputComponentEntity);
            var moveDelta = inputComponent.MoveDelta;

            DirectionEnableActiveFlag determinedRawInputFlag = DirectionEnableActiveFlag.Nothing;
            var absDelta = math.abs(moveDelta);
            var isUpDownDominant = absDelta.y > absDelta.x;
            if (isUpDownDominant)
            {
                if (absDelta.y < threshold.Vertical) return;
                // Debug.Log($"absDelta.y {absDelta.y}, threshold.Vertical {threshold.Vertical}");
                if (moveDelta.y > 0) determinedRawInputFlag |= DirectionEnableActiveFlag.IsUp;
                else if (moveDelta.y < 0) determinedRawInputFlag |= DirectionEnableActiveFlag.IsDown;
            }
            else
            {
                if (absDelta.x < threshold.Horizontal) return;
                // Debug.Log($"absDelta.x  {absDelta.x}, threshold.Horizontal {threshold.Horizontal}");
                if (moveDelta.x > 0) determinedRawInputFlag |= DirectionEnableActiveFlag.IsRight;
                else if (moveDelta.x < 0) determinedRawInputFlag |= DirectionEnableActiveFlag.IsLeft;
            }

            new MoveDirectionIJobEntity()
            {
                RawInputCommand = determinedRawInputFlag
            }.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}