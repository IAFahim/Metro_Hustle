using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using _src.Scripts.InputControls.InputControls.Data.enums;
using BovineLabs.Core;
using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.InputControls.InputControls.Data
{
    [ChangeFilterTracking]
    [BurstCompile]
    public struct DirectionInputEnableActiveComponent : IComponentData
    {
        public InputDirectionFlag Flag;

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool HasFlagFast(InputDirectionFlag target) => (Flag & target) != 0;

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool HasFlagsFast(InputDirectionFlag targets) => (Flag & targets) == targets;
        

        public readonly string ToStringFormatted()
        {
            if (Flag == InputDirectionFlag.Nothing)
            {
                return "Nothing";
            }

            var sb = new StringBuilder();
            var enabledParts = new List<string>();
            var activeParts = new List<string>();

            if (HasFlagsFast(InputDirectionFlag.UpEnable)) enabledParts.Add("Up");
            if (HasFlagsFast(InputDirectionFlag.DownEnable)) enabledParts.Add("Down");
            if (HasFlagsFast(InputDirectionFlag.LeftEnable)) enabledParts.Add("Left");
            if (HasFlagsFast(InputDirectionFlag.RightEnable)) enabledParts.Add("Right");

            if (HasFlagsFast(InputDirectionFlag.UpActive)) activeParts.Add("Up");
            if (HasFlagsFast(InputDirectionFlag.DownActive)) activeParts.Add("Down");
            if (HasFlagsFast(InputDirectionFlag.LeftActive)) activeParts.Add("Left");
            if (HasFlagsFast(InputDirectionFlag.RightActive)) activeParts.Add("Right");

            bool hasEnabled = enabledParts.Count > 0;
            bool hasActive = activeParts.Count > 0;

            if (hasEnabled)
            {
                sb.Append("Enabled: [");
                sb.Append(string.Join(", ", enabledParts));
                sb.Append("]");
            }

            if (hasEnabled && hasActive)
            {
                sb.Append(", ");
            }

            if (hasActive)
            {
                sb.Append("Active: [");
                sb.Append(string.Join(", ", activeParts));
                sb.Append("]");
            }

            return sb.ToString();
        }
    }
}