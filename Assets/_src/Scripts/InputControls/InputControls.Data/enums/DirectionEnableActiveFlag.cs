using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Burst;

namespace _src.Scripts.InputControls.InputControls.Data.enums
{
    [Flags]
    public enum DirectionEnableActiveFlag : byte
    {
        Nothing = 0b0000_0000,
        
        LeftEnable = 0b1000_0000,
        UpEnable = 0b0100_0000,
        DownEnable = 0b0010_0000,
        RightEnable = 0b0001_0000,

        IsLeft = 0b0000_1000,
        IsUp = 0b0000_0100,
        IsDown = 0b0000_0010,
        IsRight = 0b0000_0001,

        IsRightEnabledAndActive = RightEnable | IsRight,
        IsUpEnabledAndActive = UpEnable | IsUp,
        IsDownEnabledAndActive = DownEnable | IsDown,
        IsLeftEnabledAndActive = LeftEnable | IsLeft,

        ActiveStateFlagsMask = IsRight | IsUp | IsDown | IsLeft,
        EnableFlagsMask = UpEnable | DownEnable | LeftEnable | RightEnable,
    }

    [BurstCompile]
    public static class DirectionFlagEx
    {
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlagFast(this DirectionEnableActiveFlag value, DirectionEnableActiveFlag enableActiveFlag) => (value & enableActiveFlag) != 0;


        public static string ToStringFormatted(this DirectionEnableActiveFlag value)
        {
            if (value == DirectionEnableActiveFlag.Nothing)
            {
                return "Nothing";
            }

            var sb = new StringBuilder();
            var enabledParts = new List<string>();
            var activeParts = new List<string>();

            if (value.HasFlagFast(DirectionEnableActiveFlag.UpEnable)) enabledParts.Add("Up");
            if (value.HasFlagFast(DirectionEnableActiveFlag.DownEnable)) enabledParts.Add("Down");
            if (value.HasFlagFast(DirectionEnableActiveFlag.LeftEnable)) enabledParts.Add("Left");
            if (value.HasFlagFast(DirectionEnableActiveFlag.RightEnable)) enabledParts.Add("Right");

            if (value.HasFlagFast(DirectionEnableActiveFlag.IsUp)) activeParts.Add("Up");
            if (value.HasFlagFast(DirectionEnableActiveFlag.IsDown)) activeParts.Add("Down");
            if (value.HasFlagFast(DirectionEnableActiveFlag.IsLeft)) activeParts.Add("Left");
            if (value.HasFlagFast(DirectionEnableActiveFlag.IsRight)) activeParts.Add("Right");

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