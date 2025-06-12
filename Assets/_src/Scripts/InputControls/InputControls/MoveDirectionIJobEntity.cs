using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.enums;
using Unity.Burst;
using Unity.Burst.CompilerServices;
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
            var clearedFlag = directionInputEnableActive.Flag & InputDirectionFlag.Clear;

            if (Hint.Unlikely(playerInputEnableTag.ValueRO))
                directionInputEnableActive.Flag = clearedFlag | PlayerInputDirection;
            else directionInputEnableActive.Flag = clearedFlag;

            if (directionInputEnableActive.HasFlagsFast(InputDirectionFlag.UpDownActive))
            {
                directionInputEnableActive.Flag &= ~InputDirectionFlag.UpDownActive;
            }
            else if (directionInputEnableActive.HasFlagFast(InputDirectionFlag.UpActive))
            {
                directionInputEnableActive.Flag &= ~InputDirectionFlag.DownActive;
            }
            else if (directionInputEnableActive.HasFlagFast(InputDirectionFlag.DownActive))
            {
                directionInputEnableActive.Flag &= ~InputDirectionFlag.UpActive;
            }
        }
    }
}