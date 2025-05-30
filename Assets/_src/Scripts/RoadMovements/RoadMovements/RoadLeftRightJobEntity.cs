using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.enums;
using _src.Scripts.Positioning.Positioning.Data;
using _src.Scripts.RoadMovements.RoadMovements.Data;
using _src.Scripts.ZBuildings.ZBuildings.Data;
using BovineLabs.Core;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.RoadMovements.RoadMovements
{
    [BurstCompile]
    public partial struct RoadLeftRightJobEntity : IJobEntity
    {
        public RoadComponent Road;
        public BLLogger Log;

        private void Execute(
            ref LeftRightComponent leftRight,
            ref RoadMovementComponent movement, in DirectionInputEnableActiveComponent directionInput,
            in LocalToWorld localToWorld
        )
        {
            if (leftRight.Direction != (half)0) return;

            bool rightInputActive = directionInput.HasFlagFast(DirectionEnableActiveFlag.IsRightEnabledAndActive);
            bool leftInputActive = directionInput.HasFlagFast(DirectionEnableActiveFlag.IsLeftEnabledAndActive);

            if (!(rightInputActive || leftInputActive)) return;

            float targetPositionX = localToWorld.Value.c3.x;
            bool moveInitiated = false;
            var newTargetFlag = movement.CurrentRoadFlag;

            if (rightInputActive)
            {
                newTargetFlag = Road.GetAdjacentPosition(movement.CurrentRoadFlag, true, out var newPositionX);
                if (newTargetFlag != movement.CurrentRoadFlag)
                {
                    targetPositionX = newPositionX;
                    leftRight.Direction = (half)1;
                    moveInitiated = true;
                }
            }
            else if (leftInputActive)
            {
                newTargetFlag = Road.GetAdjacentPosition(movement.CurrentRoadFlag, false, out var newPositionX);
                if (newTargetFlag != movement.CurrentRoadFlag)
                {
                    targetPositionX = newPositionX;
                    leftRight.Direction = (half)(-1);
                    moveInitiated = true;
                }
            }

            if (moveInitiated)
            {
                leftRight.Current = (half)localToWorld.Value.c3.x;
                leftRight.Target = (half)targetPositionX;
                leftRight.Step = (half)10f;
                movement.CurrentRoadFlag = newTargetFlag;
            }
        }
    }
}