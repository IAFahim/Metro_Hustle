using System;

namespace _src.Scripts.Collisions.Collisions.Data.enums
{
    [Flags]
    public enum CollisionHint : ushort
    {
        Nothing = 0,
        ClearLine = 1 << 0,
        Slide = 1 << 1,
        Jump = 1 << 2,
        HideBehind = 1 << 3,
        Collect = 1 << 4,
        AvoidCollect = 1 << 5,
        WillFall = 1 << 6,
        WallRun = 1 << 7,
        SlowDown = 1 << 8,
        SpeedUp = 1 << 9,
    }
}