using System;

namespace _src.Scripts.CollisionHints.CollisionHints.Data.enums
{
    [Flags]
    public enum CollisionHint : byte
    {
        Nothing = 0,
        Hit = 1 << 1,
        Fallen = 1 << 2,
        Caught = 1 << 3,
        Collected = 1 << 4
    }
}