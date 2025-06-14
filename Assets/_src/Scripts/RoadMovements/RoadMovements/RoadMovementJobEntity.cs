using _src.Scripts.Animations.Animations.Data;
using _src.Scripts.Animations.Animations.Data.enums;
using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.enums;
using _src.Scripts.Positioning.Positioning.Data;
using _src.Scripts.RoadMovements.RoadMovements.Data;
using _src.Scripts.StatsHelpers.StatsHelpers.Data;
using _src.Scripts.ZBuildings.ZBuildings.Data;
using BovineLabs.Stats.Data;
using Unity.Burst;
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
                if (downInputActive)
                {
                    gravity.GMultiplier = (byte)(gravity.GMultiplier * 2);
                }    
            }


            


            bool rightInputActive = directionInput.HasFlagsFast(InputDirectionFlag.RightEnabledAndActive);
            bool leftInputActive = directionInput.HasFlagsFast(InputDirectionFlag.LeftEnabledAndActive);

            if (!(rightInputActive || leftInputActive)) return;

            float currentEntityX = ltw.Value.c3.x;
            bool movementParamsUpdated = false;

            var newCalculatedDirection = leftRight.Direction;
            float newCalculatedTargetX = leftRight.Target;
            var newCalculatedRoadFlag = movement.CurrentRoadFlag;


            if (leftRight.Direction == 1 && leftInputActive)
            {
                var potentialSwitchFlag =
                    Road.GetAdjacentPosition(movement.CurrentRoadFlag, false, out var potentialSwitchX);
                if (potentialSwitchFlag != movement.CurrentRoadFlag)
                {
                    newCalculatedDirection = -1;
                    newCalculatedTargetX = potentialSwitchX;
                    newCalculatedRoadFlag = potentialSwitchFlag;
                    movementParamsUpdated = true;
                }
            }
            else if (leftRight.Direction == -1 &&
                     rightInputActive)
            {
                var potentialSwitchFlag =
                    Road.GetAdjacentPosition(movement.CurrentRoadFlag, true, out var potentialSwitchX);
                if (potentialSwitchFlag != movement.CurrentRoadFlag)
                {
                    newCalculatedDirection = 1;
                    newCalculatedTargetX = potentialSwitchX;
                    newCalculatedRoadFlag = potentialSwitchFlag;
                    movementParamsUpdated = true;
                }
            }
            else if (leftRight.Direction == 0)
            {
                if (rightInputActive)
                {
                    var potentialNewFlag =
                        Road.GetAdjacentPosition(movement.CurrentRoadFlag, true, out var potentialNewX);
                    if (potentialNewFlag != movement.CurrentRoadFlag)
                    {
                        newCalculatedDirection = 1;
                        newCalculatedTargetX = potentialNewX;
                        newCalculatedRoadFlag = potentialNewFlag;
                        movementParamsUpdated = true;
                    }
                }
                else
                {
                    var potentialNewFlag =
                        Road.GetAdjacentPosition(movement.CurrentRoadFlag, false, out var potentialNewX);
                    if (potentialNewFlag != movement.CurrentRoadFlag)
                    {
                        newCalculatedDirection = -1;
                        newCalculatedTargetX = potentialNewX;
                        newCalculatedRoadFlag = potentialNewFlag;
                        movementParamsUpdated = true;
                    }
                }
            }

            if (movementParamsUpdated)
            {
                leftRight.Direction = newCalculatedDirection;
                leftRight.Current = (half)currentEntityX;
                leftRight.Target = (half)newCalculatedTargetX;
                leftRight.Speed = (half)(intrinsic.GetValue(new IntrinsicKey
                                             { Value = (ushort)EIntrinsic.LineSwitchSpeed })
                                         / 100f);
                movement.CurrentRoadFlag = newCalculatedRoadFlag;
            }
        }
    }
}