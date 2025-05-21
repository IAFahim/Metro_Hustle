using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.enums;
using BovineLabs.Core.Input;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

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

            DirectionEnableActiveFlag determinedRawInputFlag = DirectionEnableActiveFlag.Nothing;
            var absDelta = math.abs(moveDelta);
            var isUpDownDominant = absDelta.y > absDelta.x;
            if (isUpDownDominant)
            {
                if (absDelta.y < threshold.Vertical) return;
                if (moveDelta.y > 0) determinedRawInputFlag |= DirectionEnableActiveFlag.IsUp;
                else if (moveDelta.y < 0) determinedRawInputFlag |= DirectionEnableActiveFlag.IsDown;
            }
            else
            {
                if (absDelta.x < threshold.Horizontal) return;
                if (moveDelta.x > 0) determinedRawInputFlag |= DirectionEnableActiveFlag.IsRight;
                else if (moveDelta.x < 0) determinedRawInputFlag |= DirectionEnableActiveFlag.IsLeft;
            }

            MoveDirectionIJobEntity moveDirectionIJobEntity = new MoveDirectionIJobEntity()
            {
                RawInputCommand = determinedRawInputFlag
            };
            moveDirectionIJobEntity.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}