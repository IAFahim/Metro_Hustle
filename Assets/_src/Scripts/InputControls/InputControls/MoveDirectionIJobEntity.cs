using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.enums;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace _src.Scripts.InputControls.InputControls
{
    [WithAll(typeof(InputEnableTag))]
    [BurstCompile]
    public partial struct MoveDirectionIJobEntity : IJobEntity
    {
        public InputDirectionFlag PlayerInputDirection;

        [BurstCompile]
        private void Execute(ref DirectionInputEnableActiveComponent directionInputEnableActive,
            EnabledRefRO<PlayerInputEnableTag> playerInputEnableTag)
        {
            var directionFlag = directionInputEnableActive.Flag & InputDirectionFlag.Clear;
            if (playerInputEnableTag.ValueRO)
                directionInputEnableActive.Flag = directionFlag | PlayerInputDirection;
            else
                directionInputEnableActive.Flag = directionFlag;

            if ((directionInputEnableActive.Flag & InputDirectionFlag.IsUpEnabledAndActive) != 0)
            {
                directionInputEnableActive.Flag &= ~InputDirectionFlag.IsDown;
            }

            if ((directionInputEnableActive.Flag & InputDirectionFlag.IsDownEnabledAndActive) != 0)
            {
                directionInputEnableActive.Flag &= ~InputDirectionFlag.IsUp;
            }
        }
    }
}