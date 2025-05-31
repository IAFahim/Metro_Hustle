using _src.Scripts.Animations.Animations.Data;
using _src.Scripts.Animations.Animations.Data.enums;
using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.enums;
using _src.Scripts.Positioning.Positioning.Data; // Assuming this is for LeftRightComponent
using _src.Scripts.RoadMovements.RoadMovements.Data;
using _src.Scripts.StatsHelpers.StatsHelpers.Data;
using _src.Scripts.ZBuildings.ZBuildings.Data;
using BovineLabs.Stats.Data;
// using _src.Scripts.ZBuildings.ZBuildings.Data; // Not used in this specific job
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.RoadMovements.RoadMovements
{
    [BurstCompile]
    public partial struct RoadLeftRightJobEntity : IJobEntity
    {
        public RoadComponent Road;
        [ReadOnly] public BufferLookup<Stat> StatsLookup;

        private void Execute(
            Entity entity,
            ref LeftRightComponent leftRight,
            ref GravityEnableComponent gravity,
            ref RoadMovementComponent movement,
            in DirectionInputEnableActiveComponent directionInput,
            ref AnimatorComponent animatorComponent,
            ref LocalToWorld localToWorld,
            ref ForwardBackComponent forwardBackComponent
            )
        {
            var statKey = new StatKey()
            {
                Value = (ushort)EStat.ForwardSpeed
            };
            forwardBackComponent.Offset = (half)StatsLookup[entity].AsMap().Get(statKey).Value;
            bool jumpInputActive = directionInput.HasFlagFast(DirectionEnableActiveFlag.IsUpEnabledAndActive);
            if (jumpInputActive)
            {
                gravity.Enable = true;
                gravity.Velocity = (half)10;
                gravity.GravityMul = 1;
                animatorComponent.CurrentState = (sbyte)EAnimation.Jumping;
                animatorComponent.OldState = 0;
            }

            if (localToWorld.Value.c3.y < 0)
            {
                localToWorld.Value.c3.y = 0;
                gravity.Enable = false;
                gravity.Velocity = (half)0;

                animatorComponent.CurrentState = (sbyte)EAnimation.Running;
            }

            bool downInputActive = directionInput.HasFlagFast(DirectionEnableActiveFlag.IsDownEnabledAndActive);
            if (downInputActive)
            {
                gravity.GravityMul = 2;
            }


            bool rightInputActive = directionInput.HasFlagFast(DirectionEnableActiveFlag.IsRightEnabledAndActive);
            bool leftInputActive = directionInput.HasFlagFast(DirectionEnableActiveFlag.IsLeftEnabledAndActive);

            // If no relevant input, do nothing further in this job.
            // The entity will continue its current movement (or stay still) based on LeftRightSystem.
            if (!(rightInputActive || leftInputActive)) return;

            float currentEntityX = localToWorld.Value.c3.x;
            bool movementParamsUpdated = false;

            // Temporary variables to hold potential new state
            half newCalculatedDirection = leftRight.Direction;
            float newCalculatedTargetX = leftRight.Target; // Initialize with current target
            var newCalculatedRoadFlag = movement.CurrentRoadFlag;


            // 1. Check for SWITCHING direction
            if (leftRight.Direction == (half)1 && leftInputActive) // Currently moving Right, but Left input is given
            {
                var potentialSwitchFlag =
                    Road.GetAdjacentPosition(movement.CurrentRoadFlag, false, out var potentialSwitchX);
                if (potentialSwitchFlag != movement.CurrentRoadFlag) // Check if a valid left lane exists
                {
                    newCalculatedDirection = (half)(-1);
                    newCalculatedTargetX = potentialSwitchX;
                    newCalculatedRoadFlag = potentialSwitchFlag;
                    movementParamsUpdated = true;
                }
                // If no valid left lane, it will continue moving right (or complete current move)
            }
            else if (leftRight.Direction == (half)(-1) &&
                     rightInputActive) // Currently moving Left, but Right input is given
            {
                var potentialSwitchFlag =
                    Road.GetAdjacentPosition(movement.CurrentRoadFlag, true, out var potentialSwitchX);
                if (potentialSwitchFlag != movement.CurrentRoadFlag) // Check if a valid right lane exists
                {
                    newCalculatedDirection = (half)1;
                    newCalculatedTargetX = potentialSwitchX;
                    newCalculatedRoadFlag = potentialSwitchFlag;
                    movementParamsUpdated = true;
                }
                // If no valid right lane, it will continue moving left (or complete current move)
            }
            // 2. Check for INITIATING a new movement (if not already moving and no switch occurred)
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

            // If any movement (new or switched) was determined, update the components
            if (movementParamsUpdated)
            {
                leftRight.Direction = newCalculatedDirection;
                leftRight.Current = (half)currentEntityX; // CRITICAL: Base new lerp on actual current position
                leftRight.Target = (half)newCalculatedTargetX;
                leftRight.Step = (half)10f; // Set/reset speed for the new/switched movement
                movement.CurrentRoadFlag = newCalculatedRoadFlag;
            }
            // If movementParamsUpdated is false here, it means:
            // - Input matched current movement direction (e.g., moving right, input right).
            // - Trying to move into a non-existent lane (e.g., at edge of road).
            // - No input was given (handled by the early return).
            // In these cases, the existing LeftRightComponent values remain, and another system will continue processing them.
        }
    }
}