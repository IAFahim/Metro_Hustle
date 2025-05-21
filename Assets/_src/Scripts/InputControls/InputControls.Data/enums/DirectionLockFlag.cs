using System;

namespace _src.Scripts.InputControls.InputControls.Data.enums
{
    [Flags]
    public enum DirectionLockFlag : byte
    {
        Nothing = 0b0000_0000,
        
        LockLeftOnly = 0b0000_1000,
        LockUpOnly = 0b0000_0100, 
        LockDownOnly = 0b0000_0010,
        LockRightOnly = 0b0000_0001,
    }
}