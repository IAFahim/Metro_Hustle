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
        Clear = EnableFlagsMask | IsUp | IsDown
    }
}