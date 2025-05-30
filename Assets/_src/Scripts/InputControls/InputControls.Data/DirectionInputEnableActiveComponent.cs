using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using _src.Scripts.InputControls.InputControls.Data.enums;
using BovineLabs.Core;
using Unity.Entities;

namespace _src.Scripts.InputControls.InputControls.Data
{
    [ChangeFilterTracking]
    public struct DirectionInputEnableActiveComponent : IComponentData
    {
        public DirectionEnableActiveFlag Flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool HasFlagFast(DirectionEnableActiveFlag enableActiveFlag) => (Flag & enableActiveFlag) != 0;

        public readonly string ToStringFormatted()
        {
            if (Flag == DirectionEnableActiveFlag.Nothing)
            {
                return "Nothing";
            }

            var sb = new StringBuilder();
            var enabledParts = new List<string>();
            var activeParts = new List<string>();

            if (HasFlagFast(DirectionEnableActiveFlag.UpEnable)) enabledParts.Add("Up");
            if (HasFlagFast(DirectionEnableActiveFlag.DownEnable)) enabledParts.Add("Down");
            if (HasFlagFast(DirectionEnableActiveFlag.LeftEnable)) enabledParts.Add("Left");
            if (HasFlagFast(DirectionEnableActiveFlag.RightEnable)) enabledParts.Add("Right");

            if (HasFlagFast(DirectionEnableActiveFlag.IsUp)) activeParts.Add("Up");
            if (HasFlagFast(DirectionEnableActiveFlag.IsDown)) activeParts.Add("Down");
            if (HasFlagFast(DirectionEnableActiveFlag.IsLeft)) activeParts.Add("Left");
            if (HasFlagFast(DirectionEnableActiveFlag.IsRight)) activeParts.Add("Right");

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