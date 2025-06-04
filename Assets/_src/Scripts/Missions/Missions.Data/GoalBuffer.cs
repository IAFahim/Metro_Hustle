using System;
using _src.Scripts.StatsHelpers.StatsHelpers.Data;
using Unity.Entities;

namespace _src.Scripts.Missions.Missions.Data
{
    [Serializable]
    [InternalBufferCapacity(0)]
    public struct GoalBuffer : IBufferElementData
    {
        public EIntrinsic intrinsic;
        public ObjectiveComparison comparison;
        public ushort goal;

        public bool IsComplete(int amount)
        {
            return comparison switch
            {
                ObjectiveComparison.GreaterThanOrEqual => goal <= amount,
                ObjectiveComparison.LessThanOrEqual => amount <= goal,
                ObjectiveComparison.EqualTo => amount == goal,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}