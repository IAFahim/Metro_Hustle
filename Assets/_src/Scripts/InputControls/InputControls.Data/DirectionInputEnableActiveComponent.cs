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
        public InputDirectionFlag Flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool HasFlagFast(InputDirectionFlag flag) => (Flag & flag) == flag;

        public readonly string ToStringFormatted()
        {
            if (Flag == InputDirectionFlag.Nothing)
            {
                return "Nothing";
            }

            var sb = new StringBuilder();
            var enabledParts = new List<string>();
            var activeParts = new List<string>();

            if (HasFlagFast(InputDirectionFlag.UpEnable)) enabledParts.Add("Up");
            if (HasFlagFast(InputDirectionFlag.DownEnable)) enabledParts.Add("Down");
            if (HasFlagFast(InputDirectionFlag.LeftEnable)) enabledParts.Add("Left");
            if (HasFlagFast(InputDirectionFlag.RightEnable)) enabledParts.Add("Right");

            if (HasFlagFast(InputDirectionFlag.IsUp)) activeParts.Add("Up");
            if (HasFlagFast(InputDirectionFlag.IsDown)) activeParts.Add("Down");
            if (HasFlagFast(InputDirectionFlag.IsLeft)) activeParts.Add("Left");
            if (HasFlagFast(InputDirectionFlag.IsRight)) activeParts.Add("Right");

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