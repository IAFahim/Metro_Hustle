using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.Direction;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.InputControls.InputControls
{
    [WithAll(typeof(InputEnableTag), typeof(PlayerInputEnableTag))]
    [BurstCompile]
    public partial struct MoveDirectionIJobEntity : IJobEntity
    {
        public DirectionFlag RawInputCommand;

        [BurstCompile]
        private void Execute(ref DirectionInputComponent directionInputComponent)
        {
            var directionFlag = directionInputComponent.DirectionFlag & DirectionFlag.EnableFlagsMask;
            directionInputComponent.DirectionFlag = RawInputCommand | directionFlag;
        }
    }
}