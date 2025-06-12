using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Burst;

namespace _src.Scripts.InputControls.InputControls.Data.enums
{
    [Flags]
    public enum InputDirectionFlag : byte
    {
        Nothing = 0b0000_0000,
        
        LeftEnable = 0b1000_0000,
        UpEnable = 0b0100_0000,
        DownEnable = 0b0010_0000,
        RightEnable = 0b0001_0000,

        LeftActive = 0b0000_1000,
        UpActive = 0b0000_0100,
        DownActive = 0b0000_0010,
        RightActive = 0b0000_0001,

        RightEnabledAndActive = RightEnable | RightActive,
        UpEnabledAndActive = UpEnable | UpActive,
        DownEnabledAndActive = DownEnable | DownActive,
        LeftEnabledAndActive = LeftEnable | LeftActive,
        
        UpDownActive = UpActive | DownActive,

        ActiveStateFlagsMask = RightActive | UpActive | DownActive | LeftActive,
        EnableFlagsMask = UpEnable | DownEnable | LeftEnable | RightEnable,
        Clear = EnableFlagsMask | UpActive | DownActive
    }
}