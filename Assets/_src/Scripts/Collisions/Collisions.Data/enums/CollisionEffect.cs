namespace _src.Scripts.Collisions.Collisions.Data.enums
{
    public enum CollisionEffect
    {
        Nothing = 0,
        Hit = 1 << 1,
        Fallen = 1 << 2,
        Caught = 1 << 3,
        Collected = 1 << 4
    }
}