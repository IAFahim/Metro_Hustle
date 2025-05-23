using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.enums;
using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.InputControls.InputControls
{
    [WithAll(typeof(InputEnableTag), typeof(PlayerInputEnableTag))]
    [BurstCompile]
    public partial struct MoveDirectionIJobEntity : IJobEntity
    {
        public DirectionEnableActiveFlag RawInputCommand;

        [BurstCompile]
        private void Execute(ref DirectionInputEnableActiveComponent directionInputEnableActiveComponent)
        {
            var directionFlag = directionInputEnableActiveComponent.Flag & DirectionEnableActiveFlag.EnableFlagsMask;
            directionInputEnableActiveComponent.Flag = RawInputCommand | directionFlag;
        }
    }
}