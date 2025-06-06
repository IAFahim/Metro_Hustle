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

        private void Execute(
            ref GravityComponent gravity,
            ref LeftRightComponent leftRight,
            ref RoadMovementComponent movement,
            ref AnimatorComponent animator,
            ref ForwardBackComponent forwardBack,
            ref LocalToWorld ltw,
            in DirectionInputEnableActiveComponent directionInput,
            in DynamicBuffer<Stat> statsBuffer,
            in HeightComponent height
        )
        {

            if (ltw.Value.c3.y < height.Value)
            {
                ltw.Value.c3.y = height.Value;
                gravity.GMultiplier = new half(0);
                gravity.Velocity = new half(0);
                animator.CurrentState = (sbyte)EAnimation.Running;
            }
            

            var statsMap = statsBuffer.AsMap();
            forwardBack.Offset = (half)statsMap.Get(new StatKey { Value = (ushort)EStat.ForwardSpeed }).Value;
            bool jumpInputActive = directionInput.HasFlagFast(DirectionEnableActiveFlag.IsUpEnabledAndActive);
            if (jumpInputActive && height.Value == ltw.Position.y)
            {
                var jumpForce = (half)statsMap.Get(new StatKey { Value = (ushort)EStat.JumpForce }).Value;
                gravity.Velocity += new half(jumpForce);
                gravity.GMultiplier += new half(1);
                animator.CurrentState = (sbyte)EAnimation.Jumping;
                animator.OldState = 0;
            }


            bool downInputActive = directionInput.HasFlagFast(DirectionEnableActiveFlag.IsDownEnabledAndActive);
            if (downInputActive)
            {
                gravity.GMultiplier *= new half(2);
            }


            bool rightInputActive = directionInput.HasFlagFast(DirectionEnableActiveFlag.IsRightEnabledAndActive);
            bool leftInputActive = directionInput.HasFlagFast(DirectionEnableActiveFlag.IsLeftEnabledAndActive);

            if (!(rightInputActive || leftInputActive)) return;

            float currentEntityX = ltw.Value.c3.x;
            bool movementParamsUpdated = false;

            half newCalculatedDirection = leftRight.Direction;
            float newCalculatedTargetX = leftRight.Target;
            var newCalculatedRoadFlag = movement.CurrentRoadFlag;


            if (leftRight.Direction == (half)1 && leftInputActive)
            {
                var potentialSwitchFlag =
                    Road.GetAdjacentPosition(movement.CurrentRoadFlag, false, out var potentialSwitchX);
                if (potentialSwitchFlag != movement.CurrentRoadFlag)
                {
                    newCalculatedDirection = (half)(-1);
                    newCalculatedTargetX = potentialSwitchX;
                    newCalculatedRoadFlag = potentialSwitchFlag;
                    movementParamsUpdated = true;
                }
            }
            else if (leftRight.Direction == (half)(-1) &&
                     rightInputActive)
            {
                var potentialSwitchFlag =
                    Road.GetAdjacentPosition(movement.CurrentRoadFlag, true, out var potentialSwitchX);
                if (potentialSwitchFlag != movement.CurrentRoadFlag)
                {
                    newCalculatedDirection = (half)1;
                    newCalculatedTargetX = potentialSwitchX;
                    newCalculatedRoadFlag = potentialSwitchFlag;
                    movementParamsUpdated = true;
                }
            }
            else if (leftRight.Direction == (half)0)
            {
                if (rightInputActive)
                {
                    var potentialNewFlag =
                        Road.GetAdjacentPosition(movement.CurrentRoadFlag, true, out var potentialNewX);
                    if (potentialNewFlag != movement.CurrentRoadFlag)
                    {
                        newCalculatedDirection = (half)1;
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
                        newCalculatedDirection = (half)(-1);
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
                leftRight.Step = (half)10f;
                movement.CurrentRoadFlag = newCalculatedRoadFlag;
            }
        }
    }
}