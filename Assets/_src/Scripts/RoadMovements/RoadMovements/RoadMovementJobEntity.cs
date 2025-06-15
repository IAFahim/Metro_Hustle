using System.Runtime.CompilerServices;
using _src.Scripts.Animations.Animations.Data;
using _src.Scripts.Animations.Animations.Data.enums;
using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.enums;
using _src.Scripts.Positioning.Positioning.Data;
using _src.Scripts.RoadMovements.RoadMovements.Data;
using _src.Scripts.StatsHelpers.StatsHelpers.Data;
using _src.Scripts.ZBuildings.ZBuildings.Data;
using BovineLabs.Core.Iterators;
using BovineLabs.Stats.Data;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.RoadMovements.RoadMovements
{
    [BurstCompile]
    public partial struct RoadMovementJobEntity : IJobEntity
    {
        public RoadComponent Road;

        [BurstCompile]
        private void Execute(
            ref GravityComponent gravity,
            ref LeftRightComponent leftRight,
            ref RoadMovementComponent movement,
            ref AnimatorComponent animator,
            ref ForwardBackComponent forwardBack,
            in LocalToWorld ltw,
            in DirectionInputEnableActiveComponent directionInput,
            in DynamicBuffer<Intrinsic> intrinsicBuffer,
            in HeightComponent height
        )
        {
            if (gravity.GMultiplier == 0)
            {
                animator.CurrentState = (byte)EAnimation.Running;
            }


            var intrinsic = intrinsicBuffer.AsMap();
            forwardBack.Speed = (half)(intrinsic.GetValue(new IntrinsicKey { Value = (ushort)EIntrinsic.ForwardSpeed })
                                       / 100f);
            bool jumpInputActive = directionInput.HasFlagsFast(InputDirectionFlag.UpEnabledAndActive);
            if (jumpInputActive && height.Value == ltw.Value.c3.y)
            {
                var jumpForce = (half)(intrinsic.GetValue(new IntrinsicKey { Value = (ushort)EIntrinsic.JumpForce }) /
                                       100f);
                gravity.Velocity += jumpForce;
                gravity.GMultiplier += 1;
                animator.OldState = 0;
                animator.CurrentState = (byte)EAnimation.Jumping;
            }
            else
            {
                bool downInputActive = directionInput.HasFlagsFast(InputDirectionFlag.DownEnabledAndActive);
                if (downInputActive) gravity.GMultiplier = (byte)(gravity.GMultiplier * 2);
            }

            HandleLeftRight(directionInput, ltw, ref leftRight, ref movement, intrinsic);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void HandleLeftRight(
            in DirectionInputEnableActiveComponent directionInput,
            in LocalToWorld ltw,
            ref LeftRightComponent leftRight,
            ref RoadMovementComponent movement,
            in DynamicHashMap<IntrinsicKey, int> intrinsic
        )
        {
            bool rightInputActive = directionInput.HasFlagsFast(InputDirectionFlag.RightEnabledAndActive);
            bool leftInputActive = directionInput.HasFlagsFast(InputDirectionFlag.LeftEnabledAndActive);
            if (!(rightInputActive || leftInputActive)) return;

            float currentEntityX = ltw.Value.c3.x;
            bool movementParamsUpdated = false;

            var direction = leftRight.GetDirection();
            float newCalculatedTargetX = leftRight.Target;
            var newCalculatedRoadFlag = movement.CurrentRoadFlag;


            if (direction == 1 && leftInputActive)
            {
                var potentialSwitchFlag =
                    Road.GetAdjacentPosition(movement.CurrentRoadFlag, false, out var potentialSwitchX);
                if (potentialSwitchFlag != movement.CurrentRoadFlag)
                {
                    newCalculatedTargetX = potentialSwitchX;
                    newCalculatedRoadFlag = potentialSwitchFlag;
                    movementParamsUpdated = true;
                }
            }
            else if (direction == -1 && rightInputActive)
            {
                var potentialSwitchFlag =
                    Road.GetAdjacentPosition(movement.CurrentRoadFlag, true, out var potentialSwitchX);
                if (potentialSwitchFlag != movement.CurrentRoadFlag)
                {
                    newCalculatedTargetX = potentialSwitchX;
                    newCalculatedRoadFlag = potentialSwitchFlag;
                    movementParamsUpdated = true;
                }
            }
            else if (Hint.Likely(direction == 0))
            {
                if (rightInputActive)
                {
                    var nextFlag = Road.GetAdjacentPosition(movement.CurrentRoadFlag, true, out var potentialNewX);
                    if (nextFlag != movement.CurrentRoadFlag)
                    {
                        newCalculatedTargetX = potentialNewX;
                        newCalculatedRoadFlag = nextFlag;
                        movementParamsUpdated = true;
                    }
                }
                else
                {
                    var nextFlag = Road.GetAdjacentPosition(movement.CurrentRoadFlag, false, out var potentialNewX);
                    if (nextFlag != movement.CurrentRoadFlag)
                    {
                        newCalculatedTargetX = potentialNewX;
                        newCalculatedRoadFlag = nextFlag;
                        movementParamsUpdated = true;
                    }
                }
            }

            if (!movementParamsUpdated) return;
            leftRight.Current = (half)currentEntityX;
            leftRight.Target = (half)newCalculatedTargetX;
            var intrinsicKey = new IntrinsicKey { Value = (ushort)EIntrinsic.LineSwitchSpeed };
            leftRight.Speed = (half)(intrinsic.GetValue(intrinsicKey) / 100f);
            movement.CurrentRoadFlag = newCalculatedRoadFlag;
        }
    }
}